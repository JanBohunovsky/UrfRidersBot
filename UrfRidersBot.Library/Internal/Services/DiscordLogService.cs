using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace UrfRidersBot.Library.Internal.Services
{
    internal class DiscordLogService
    {
        private readonly ILogger _discordLogger;
        private readonly ILogger _commandLogger;

        public DiscordLogService(DiscordSocketClient discord, CommandService command, ILoggerFactory loggerFactory)
        {
            _discordLogger = loggerFactory.CreateLogger<DiscordSocketClient>();
            _commandLogger = loggerFactory.CreateLogger<CommandService>();

            discord.Log += LogDiscord;
            command.Log += LogCommand;
        }
        
        private static LogLevel LogLevelFromSeverity(LogSeverity severity) => (LogLevel)Math.Abs((int)severity - 5);

        private Task LogDiscord(LogMessage message)
        {
            _discordLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                (m, e) => message.ToString(prependTimestamp: false)
            );

            return Task.CompletedTask;
        }

        private Task LogCommand(LogMessage message)
        {
            _commandLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                (m, e) => message.ToString(prependTimestamp: false)
            );

            return Task.CompletedTask;
        }
    }
}