using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;
using UrfRidersBot.Commands.Parsers;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Configuration;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Infrastructure;

namespace UrfRidersBot.Commands
{
    public class CommandHandler : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly CommandService _commandService;
        private readonly IOptionsMonitor<DiscordOptions> _discordOptions;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<CommandHandler> _logger;
        private readonly IServiceProvider _provider;

        public CommandHandler(DiscordClient client,
            CommandService commandService,
            IOptionsMonitor<DiscordOptions> discordOptions, 
            IUnitOfWorkFactory unitOfWorkFactory,
            IHostEnvironment environment,
            ILogger<CommandHandler> logger,
            IServiceProvider provider)
        {
            _client = client;
            _commandService = commandService;
            _discordOptions = discordOptions;
            _unitOfWorkFactory = unitOfWorkFactory;
            _environment = environment;
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
            var result = await _commandService.ExecuteAsync(output, context);

            _logger.LogDebug("Command result: {Result}", result.GetType());
            
            if (result.IsSuccessful)
            {
                return;
            }

            switch (result)
            {
                case CheckResult checkResult:
                    _logger.LogError("Checks failed: {Reason}", checkResult.Reason);
                    break;
                case ArgumentParseFailedResult argumentResult:
                    _logger.LogError("Argument parsing failed: {Reason}", argumentResult.Reason);
                    break;
                case TypeParseFailedResult typeResult:
                    _logger.LogError("Type parsing failed for parameter '{Type} {Parameter}': {Reason}", 
                        typeResult.Parameter.Type.Name, typeResult.Parameter.Name, typeResult.Reason);
                    break;
            }
        }

        private async Task OnCommandExecutionFailed(CommandExecutionFailedEventArgs e)
        {
            var commandName = e.Result.Command.Name ?? "unknown command";
            var step = e.Result.CommandExecutionStep;
            var reason = e.Result.Reason;
            var exception = e.Result.Exception;
            
            _logger.LogError(exception, "Command '{CommandName}' errored at step {Step} with reason '{Reason}'", 
                commandName, step, reason);

            // Send message to bot owner if in production
            if (!_environment.IsProduction())
            {
                return;
            }

            var context = (UrfRidersCommandContext)e.Context;
            var owner = await context.Guild.GetMemberAsync(context.Client.CurrentApplication.Owners.First().Id);

            if (owner is null)
            {
                return;
            }
            
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Red,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = "An exception has occured",
                    IconUrl = UrfRidersIcon.HighPriority,
                },
                Description = $"Here is the [message]({context.Message.JumpLink}) that caused this exception:\n{Markdown.CodeBlock(exception.ToString())}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"{context.User.Username}#{context.User.Discriminator}",
                    IconUrl = context.User.GetAvatarUrl(ImageFormat.Auto),
                },
                Timestamp = context.Message.Timestamp,
            };

            embed
                .AddField("Guild", context.Guild.Name, true)
                .AddField("Channel", context.Channel.Name, true)
                .AddField("Message", Markdown.Code(context.Message.Content), true);

            await owner.SendMessageAsync(embed);
        }
    }
}