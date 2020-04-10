using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UrfRiders.Util;

namespace UrfRiders.Services
{
    public class LogService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ILogger _discordLogger;
        private readonly ILogger _commandsLogger;

        public LogService(DiscordSocketClient client, CommandService commands, ILoggerFactory loggerFactory)
        {
            _client = client;
            _commands = commands;

            _discordLogger = loggerFactory.CreateLogger("discord");
            _commandsLogger = loggerFactory.CreateLogger("commands");

            _client.Log += LogDiscord;
            _commands.Log += LogCommand;
        }

        private Task LogDiscord(LogMessage message)
        {
            _discordLogger.Log(
                LogLevelFromSeverity(message.Severity), 
                0, 
                message, 
                message.Exception,
                (m, e) => message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        private Task LogCommand(LogMessage message)
        {
#if RELEASE
            // Send me a DM when exception happens
            if (message.Exception is CommandException command)
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithTitle("Command Exception")
                    .WithThumbnailUrl(command.Context.Guild.IconUrl)
                    .WithFooter(command.Context.User.ToString(), command.Context.User.GetAvatarUrl())
                    .WithTimestamp(command.Context.Message.Timestamp)
                    .AddField("Guild", $"{command.Context.Guild.Name} (`{command.Context.Guild.Id}`)", true)
                    .AddField("Channel", $"{command.Context.Channel.Name} (`{command.Context.Channel.Id}`)", true)
                    .AddField("Message", $"`{command.Context.Message.Content}`")
                    .AddField("Exception", command.InnerException?.ToString().ToCode(true))
                    .Build();

                Task.Run(async () =>
                {
                    var appInfo = await _client.GetApplicationInfoAsync();
                    await appInfo.Owner.SendMessageAsync(embed: embed);
                });
            }
#endif

            _commandsLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                (m, e) => message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        private static LogLevel LogLevelFromSeverity(LogSeverity severity) => (LogLevel)(Math.Abs((int)severity - 5));
    }
}