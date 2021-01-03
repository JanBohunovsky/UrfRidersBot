using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace UrfRidersBot
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _command;
        private readonly BotConfiguration _botConfig;
        private readonly EmbedService _embed;
        private readonly IServiceProvider _provider;
        private readonly IHostEnvironment _environment;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;
        private readonly HelpService _helpService;

        public CommandHandlingService(
            DiscordSocketClient client,
            CommandService command,
            BotConfiguration botConfig,
            EmbedService embed,
            IServiceProvider provider,
            IHostEnvironment environment,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory,
            HelpService helpService)
        {
            _client = client;
            _command = command;
            _botConfig = botConfig;
            _embed = embed;
            _provider = provider;
            _environment = environment;
            _dbContextFactory = dbContextFactory;
            _helpService = helpService;

            _client.MessageReceived += OnMessageReceived;
            _command.CommandExecuted += OnCommandExecuted;
        }

        public async Task RegisterCommands(Assembly? assembly)
        {
            await _command.AddModulesAsync(assembly, _provider);
        }

        private async Task OnMessageReceived(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message))
                return;
            if (message.Source != MessageSource.User)
                return;

            // Default prefix.
            var prefix = _botConfig.Prefix;
            
            // Use guild's custom prefix if they have one.
            if (message.Channel is SocketGuildChannel channel)
            {
                await using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var settings = await dbContext.GuildSettings.FindAsync(channel.Guild.Id);
                    if (settings?.CustomPrefix != null)
                    {
                        prefix = settings.CustomPrefix;
                    }
                }
            }
            
            // Check if message uses a valid prefix and try to execute a command.
            int argPos = 0;
            if (message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
                message.HasStringPrefix(prefix, ref argPos))
            {
                var context = new UrfRidersContext(_client, message, prefix);
                await _command.ExecuteAsync(context, argPos, _provider);
            }
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext rawContext, IResult result)
        {
            if (!command.IsSpecified)
                return;
            if (result.IsSuccess)
                return;

            // Context should always be of type UrfRidersContext and if not then something must've gone terribly wrong
            // or someone just forgot to use the correct one.
            // In both cases, it's good to throw an InvalidCastException.
            var context = (UrfRidersContext)rawContext;

            EmbedBuilder? embedBuilder;
            if (result.Error == CommandError.Exception)
            {
                // Show a special message on command exception.
                embedBuilder = _embed
                    .CreateCriticalError("An exception has occured.")
                    .WithFooter("The bot owner has been notified.");

                // Send a message to bot owner
                if (_environment.IsProduction())
                {
                    var embed = _embed
                        .CreateCriticalError("Check logs for more information.", "An exception has occured")
                        .AddField("Guild", $"{context.Guild.Name}", true)
                        .AddField("Channel", $"{context.Channel.Name}", true)
                        .AddField("Message", $"`{context.Message.Content}` - [link]({context.Message.GetJumpUrl()})", true)
                        .WithFooter(context.User.ToString(), context.User.GetAvatarUrl())
                        .WithTimestamp(context.Message.Timestamp)
                        .Build();

                    _ = Task.Run(async () =>
                    {
                        var appInfo = await _client.GetApplicationInfoAsync();
                        await appInfo.Owner.SendMessageAsync(embed: embed);
                    });
                }
            }
            else
            {
                // Show generic error message based on the error type.
                embedBuilder = _embed.CreateError(result.ErrorReason);
            }

            // If the command syntax wasn't correct, show proper command usage.
            if (result.Error == CommandError.BadArgCount)
            {
                embedBuilder.AddField("Usage", _helpService.GetCommandUsage(command.Value, context.Prefix));
            }

            await context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }
}