using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace UrfRidersBot.Library.Internal.Services
{
    internal class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _command;
        private readonly BotConfiguration _botConfig;
        private readonly IEmbedService _embed;
        private readonly IServiceProvider _provider;
        private readonly IHostEnvironment _environment;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;

        public CommandHandlingService(DiscordSocketClient discord, CommandService command, BotConfiguration botConfig,
            IEmbedService embed, IServiceProvider provider, IHostEnvironment environment,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory)
        {
            _discord = discord;
            _command = command;
            _botConfig = botConfig;
            _embed = embed;
            _provider = provider;
            _environment = environment;
            _dbContextFactory = dbContextFactory;

            _discord.MessageReceived += MessageReceived;
            _command.CommandExecuted += CommandExecuted;
        }

        public async Task RegisterCommands(Assembly? assembly)
        {
            await _command.AddModulesAsync(assembly, _provider);
        }

        private async Task MessageReceived(SocketMessage rawMessage)
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
            if (message.HasMentionPrefix(_discord.CurrentUser, ref argPos) ||
                message.HasStringPrefix(prefix, ref argPos))
            {
                var context = new UrfRidersContext(_discord, message);
                await _command.ExecuteAsync(context, argPos, _provider);
            }
        }

        private async Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
                return;
            if (result.IsSuccess)
                return;

            EmbedBuilder? embedBuilder;
            if (result.Error == CommandError.Exception)
            {
                embedBuilder = _embed
                    .CriticalError($"An exception has occured.")
                    .WithFooter("The bot owner has been notified.");

                // Send a message to bot owner
                if (_environment.IsProduction())
                {
                    var embed = _embed.CriticalError("Check logs for more information.", "An exception has occured")
                        .AddField("Guild", $"{context.Guild.Name}", true)
                        .AddField("Channel", $"{context.Channel.Name}", true)
                        .AddField("Message", $"`{context.Message.Content}` - [link]({context.Message.GetJumpUrl()})",
                            true)
                        .WithFooter(context.User.ToString(), context.User.GetAvatarUrl())
                        .WithTimestamp(context.Message.Timestamp)
                        .Build();

                    _ = Task.Run(async () =>
                    {
                        var appInfo = await _discord.GetApplicationInfoAsync();
                        await appInfo.Owner.SendMessageAsync(embed: embed);
                    });
                }
            }
            else
            {
                embedBuilder = _embed.Error(result.ErrorReason);
            }

            await context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }
}