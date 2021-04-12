using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Built;
using UrfRidersBot.Core.Commands.Services;
using UrfRidersBot.Infrastructure.Commands.Models;

namespace UrfRidersBot.Infrastructure.Commands.Services
{
    internal class CommandHandler : ICommandHandler
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordClient _client;
        private Dictionary<string, SlashCommand> _commands;

        public CommandHandler(
            IServiceProvider provider,
            DiscordClient client)
        {
            _provider = provider;
            _client = client;
            _commands = new Dictionary<string, SlashCommand>();
        }

        public void AddCommands(IEnumerable<SlashCommand> commands)
        {
            _commands = commands.ToDictionary(c => c.GetFullName(), c => c);
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            if (interaction.Guild is null)
            {
                throw new Exception("User tried to execute a command in DMs, somehow.");
            }
            
            var request = CommandRequest.FromInteractionData(interaction.Data);
            
            if (!_commands.ContainsKey(request.FullName))
            {
                throw new Exception($"An interaction was created, but no command was registered for it: '{request.FullName}'");
            }
            var command = _commands[request.FullName];

            using var scope = _provider.CreateScope();
            if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, command.Class) is not ICommand instance)
            {
                throw new Exception($"Could not create an instance of a type {command.Class} for a command '{request.FullName}'.");
            }
            
            var context = new CommandContext(_client, interaction, instance.Ephemeral);
            await AcknowledgeInteraction(interaction, instance.Ephemeral);

            CommandResult result;
            try
            {
                if (request.Parameters is not null)
                {
                    command.FillParameters(instance, request.Parameters, interaction.Data.Resolved);
                }
                
                var checkResult = await command.RunChecksAsync(context, _provider);
                if (!checkResult.IsSuccessful)
                {
                    await context.RespondWithAccessDeniedAsync(checkResult.Reason);
                    return;
                }

                result = await instance.HandleAsync(context);
            }
            catch (Exception e)
            {
                await context.RespondWithCriticalErrorAsync(e);
                throw;
            }

            if (instance.Ephemeral)
            {
                await RespondWithMessageAsync(context, result);
            }
            else
            {
                await RespondWithEmbedAsync(context, result);
            }
        }

        private async Task AcknowledgeInteraction(DiscordInteraction interaction, bool ephemeral)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .AsEphemeral(ephemeral);
            
            await interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, builder);
        }

        private async Task RespondWithMessageAsync(ICommandContext context, CommandResult result)
        {
            string? content = result.Type switch
            {
                CommandResultType.Success          => $"{UrfRidersEmotes.Checkmark} {result.Message ?? "Success"}",
                CommandResultType.InvalidOperation => $"{UrfRidersEmotes.Error} Error: {result.Message}",
                CommandResultType.InvalidParameter => $"{UrfRidersEmotes.Error} Parameter error: {result.Message}",
                _                                  => null
            };

            if (content is not null)
            {
                await context.RespondAsync(content);
            }
        }

        private async Task RespondWithEmbedAsync(ICommandContext context, CommandResult result)
        {
            DiscordEmbedBuilder? embed = result.Type switch
            {
                CommandResultType.Success          => EmbedHelper.CreateSuccess(result.Message),
                CommandResultType.InvalidOperation => EmbedHelper.CreateError(result.Message),
                CommandResultType.InvalidParameter => EmbedHelper.CreateError(result.Message, "Parameter error"),
                _                                  => null
            };

            if (embed is not null)
            {
                await context.RespondAsync(embed);
            }
        }
    }
}