using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace UrfRiders.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly CommandHelper _helper;
        private readonly LiteDatabase _database;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services, DiscordSocketClient client, CommandService commands, CommandHelper helper, LiteDatabase database)
        {
            _client = client;
            _commands = commands;
            _helper = helper;
            _database = database;
            _services = services;

            _client.MessageReceived += MessageReceived;
            _commands.CommandExecuted += CommandExecuted;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            var prefix = ServerSettings.Default.Prefix;
            if (message.Channel is IGuildChannel channel)
            {
                prefix = new ServerSettings(channel.GuildId, _database).Prefix;
            }
            if (!message.HasMentionPrefix(_client.CurrentUser, ref argPos) && !message.HasStringPrefix(prefix, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
                return;
            if (result.IsSuccess)
                return;

            if (result.Error == CommandError.Exception)
            {
                var embed = new EmbedBuilder()
                    .WithError("An exception has occured. Please contact bot owner.")
                    .WithColor(Color.Red)
                    .Build();
                await context.Channel.SendMessageAsync(embed: embed);
                return;
            }

            if (result.Error == CommandError.BadArgCount)
            {
                if (command.Value.Name == "ModuleHelp")
                {
                    // Unknown sub-command
                    await context.Channel.SendMessageAsync(embed: new EmbedBuilder().WithError("Unknown command.").Build());
                    return;
                }
                var settings = new ServerSettings(context.Guild.Id, _database);

                var embed = new EmbedBuilder()
                    .WithError(result.ErrorReason)
                    .AddField("Command usage", _helper.Usage(command.Value).ToCode(settings.LargeCodeBlock))
                    .Build();
                await context.Channel.SendMessageAsync(embed: embed);
                return;
            }

            await context.Channel.SendMessageAsync(embed: new EmbedBuilder().WithError(result.ErrorReason).Build());
        }
    }
}