using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;
using UrfRidersBot.Commands.Parsers;
using UrfRidersBot.Core.Configuration;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Commands
{
    public class CommandHandler : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly CommandService _commandService;
        private readonly IOptionsMonitor<DiscordOptions> _discordOptions;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger<CommandHandler> _logger;
        private readonly IServiceProvider _provider;

        public CommandHandler(DiscordClient client,
            CommandService commandService,
            IOptionsMonitor<DiscordOptions> discordOptions, 
            IUnitOfWorkFactory unitOfWorkFactory,
            ILogger<CommandHandler> logger,
            IServiceProvider provider)
        {
            _client = client;
            _commandService = commandService;
            _discordOptions = discordOptions;
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
            _provider = provider;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _commandService.AddModules(Assembly.GetExecutingAssembly());
            _commandService.AddTypeParser(new DiscordUserTypeParser());
            _commandService.AddTypeParser(new DiscordMemberTypeParser());
            _commandService.AddTypeParser(new DiscordRoleTypeParser());
            _commandService.AddTypeParser(new DiscordChannelTypeParser());
            _commandService.AddTypeParser(new DiscordGuildTypeParser());
            _commandService.AddTypeParser(new DiscordMessageTypeParser());
            _commandService.AddTypeParser(new DiscordEmojiTypeParser());
            _commandService.AddTypeParser(new DiscordColorTypeParser());
            
            _client.MessageCreated += OnMessageCreated;
            _commandService.CommandExecutionFailed += OnCommandExecutionFailed;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.MessageCreated -= OnMessageCreated;
            _commandService.CommandExecutionFailed -= OnCommandExecutionFailed;
            
            _commandService.RemoveAllTypeParsers();
            _commandService.RemoveAllModules();

            return Task.CompletedTask;
        }

        private async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            // Ignore bots
            if (e.Author.IsBot)
                return;
            
            // Ignore DMs
            if (e.Channel.IsPrivate)
                return;
            
            var prefixes = new List<string>();
            
            // Add default or custom prefix
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var settings = await unitOfWork.GuildSettings.GetAsync(e.Guild);
            prefixes.Add(settings?.CustomPrefix ?? _discordOptions.CurrentValue.Prefix);

            // Add mention prefix
            prefixes.Add(Formatter.Mention(_client.CurrentUser, false));
            prefixes.Add(Formatter.Mention(_client.CurrentUser, true));
            
            // Try prefix or mention
            if (!CommandUtilities.HasAnyPrefix(e.Message.Content, prefixes, out var prefix, out var output))
                return;
            
            // Execute command
            var context = new UrfRidersCommandContext(e.Message, sender, prefix, _provider);
            await _commandService.ExecuteAsync(output, context);
        }

        private Task OnCommandExecutionFailed(CommandExecutionFailedEventArgs e)
        {
            var commandName = e.Result.Command.Name ?? "unknown command";
            var step = e.Result.CommandExecutionStep;
            var reason = e.Result.Reason;
            var exception = e.Result.Exception;
            
            _logger.LogError(exception, "Command '{CommandName}' errored at step {Step} with reason '{Reason}'", 
                commandName, step, reason);

            return Task.CompletedTask;
        }
    }
}