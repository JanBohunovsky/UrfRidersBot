using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UrfRidersBot.Library
{
    internal class HelpService : IHelpService
    {
        private readonly IEmbedService _embed;
        private readonly CommandService _command;
        private readonly BotConfiguration _botConfig;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _provider;
        
        public HelpService(
            IEmbedService embed,
            CommandService command,
            BotConfiguration botConfig,
            DiscordSocketClient discord,
            IServiceProvider provider)
        {
            _embed = embed;
            _command = command;
            _botConfig = botConfig;
            _discord = discord;
            _provider = provider;
        }
        
        public async ValueTask<Embed> GetCommandDetails(UrfRidersContext context, string name)
        {
            var notFound = _embed.CreateError("Could not find specified command.").Build();
            var result = _embed.CreateBotInfo("Documentation");
            
            // Find the command
            var command = _command.Commands.FirstOrDefault(x => x.Aliases.Any(alias => string.Equals(alias, name, StringComparison.InvariantCultureIgnoreCase)));
            if (command == null)
                return notFound;
            
            // Check if user can use the command
            var preconditionResult = await command.CheckPreconditionsAsync(context, _provider);
            if (!preconditionResult.IsSuccess)
                return notFound;
            
            // Build the embed
            result.WithTitle(command.Name);

            if (command.Summary != null)
                result.WithDescription(command.Summary);
            
            result.AddField("Usage", GetCommandUsage(command, context.Prefix));

            // Show available aliases (without group prefix)
            if (command.Aliases.Count > 1)
            {
                result.AddField("Aliases", string.Join(", ", command.Aliases.Skip(1).Select(x => x.Substring(x.LastIndexOf(' ') + 1).ToCode())));
            }
            
            return result.Build();
        }

        public async ValueTask<Embed> GetAllCommands(UrfRidersContext context)
        {
            var result = _embed.CreateBotInfo("Documentation");

            foreach (var module in GetModules())
            {
                var field = await FormatModule(module, context);
                if (field != null)
                    result.AddField(field);
            }

            result.WithFooter($"Use '{context.Prefix}help <command>' for more details.");

            return result.Build();
        }

        public string GetCommandUsage(CommandInfo command, string? prefix = null)
        {
            return $"`{prefix}{command.Aliases.First()}{GetCommandParameters(command)}`";
        }

        /// <summary>
        /// Formats <see cref="module"/> as an <see cref="EmbedField"/> with its description and a list of commands.
        /// </summary>
        private async ValueTask<EmbedFieldBuilder?> FormatModule(ModuleInfo module, UrfRidersContext context)
        {
            var descriptionBuilder = new StringBuilder();
            if (module.Summary != null)
            {
                descriptionBuilder.AppendLine($"*{module.Summary}*");
            }
            if (module.Submodules.Any())
            {
                // TODO: Test this out and finish this method
                descriptionBuilder.AppendLine($"Submodules: {string.Join(", ", module.Submodules.Select(m => m.Name))}");
            }

            var commands = await GetExecutableCommandList(module, context).ToListAsync();
            // User can't invoke a single command -> don't show this module in help
            if (commands.Count <= 0)
                return null;
            
            descriptionBuilder.AppendJoin(" ", commands);
            
            return new EmbedFieldBuilder
            {
                Name = $"{module.Name} commands",
                Value = descriptionBuilder.ToString(),
            };
        }

        /// <summary>
        /// Returns a list of commands which can be executed by the user, formatted as code.
        /// </summary>
        private async IAsyncEnumerable<string> GetExecutableCommandList(ModuleInfo module, UrfRidersContext context)
        {
            var commands = await module.GetExecutableCommandsAsync(context, _provider);
            foreach (var commandInfo in commands)
            {
                var command = commandInfo.Aliases.First();
                
                // if (hideGroupPrefix)
                // {
                //     // Remove group prefix
                //     command = command.Substring(command.LastIndexOf(' ') + 1);
                //     
                //     // If this command is THE group command then ignore it
                //     if (module.Group == command)
                //         continue;
                // }
            
                yield return command.ToCode()!;
            }
        }
        
        private string GetCommandParameters(CommandInfo command)
        {
            var result = new StringBuilder();
            
            foreach (var parameter in command.Parameters)
            {
                result.Append(' ');
                if (parameter.IsOptional && parameter.DefaultValue != null)
                    result.Append($"[{parameter.Name} = {parameter.DefaultValue}]");
                else if (parameter.IsOptional && parameter.DefaultValue == null)
                    result.Append($"[{parameter.Name}]");
                else if (parameter.IsMultiple)
                    result.Append($"|{parameter.Name}|");
                else
                    result.Append($"<{parameter.Name}>");
            }
            
            return result.ToString();
        }

        private IEnumerable<ModuleInfo> GetModules()
        {
            return _command.Modules.Where(m => m.Parent == null && m.Commands.Count > 0);
        }
    }
}