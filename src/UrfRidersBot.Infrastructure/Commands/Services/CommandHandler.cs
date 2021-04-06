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
            var request = CommandRequest.FromInteractionData(interaction.Data);

            var command = GetCommand(request);
            
            using var scope = _provider.CreateScope();
            var instance = CreateCommandInstance(command, request, interaction.Data.Resolved, scope);

            var context = await CreateCommandContextAsync(interaction);

            await RunChecksAsync(command, context);

            try
            {
                await instance.HandleAsync(context);
            }
            catch (CommandException e)
            {
                await context.CreateEphemeralResponseAsync($"<:error:828216690452987935> Error: {e.Message}");
            }
            catch (Exception e)
            {
                await context.CreateEphemeralResponseAsync($"<:high_priority:828216690164105257> Command failed, please contact bot owner.\n" +
                                                           $"Exception: {Markdown.Code(e.Message)}");
                throw;
            }
        }

        private SlashCommand GetCommand(CommandRequest request)
        {
            if (!_commands.ContainsKey(request.FullName))
            {
                throw new Exception($"An interaction was created, but no command was registered for it: '{request.FullName}'");
            }

            return _commands[request.FullName];
        }

        private ICommand CreateCommandInstance(
            SlashCommand command,
            CommandRequest request,
            DiscordInteractionResolvedCollection resolved,
            IServiceScope scope)
        {
            if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, command.Class) is not ICommand instance)
            {
                throw new Exception($"Could not create an instance of a type {command.Class} for a command '{request.FullName}'.");
            }

            if (request.Parameters is not null)
            {
                command.FillParameters(instance, request.Parameters, resolved);
            }

            return instance;
        }

        private async ValueTask<CommandContext> CreateCommandContextAsync(DiscordInteraction interaction)
        {
            var context = new CommandContext(_client, interaction);
            if (interaction.Guild is null)
            {
                await context.CreateResponseAsync(EmbedHelper.CreateError("This command can be used only in a server."));
                throw new Exception("User tried to execute a command in DMs, somehow.");
            }

            return context;
        }

        private async Task RunChecksAsync(SlashCommand command, CommandContext context)
        {
            var checkResult = await command.RunChecksAsync(context, _provider);
            if (!checkResult.IsSuccessful)
            {
                await context.CreateEphemeralResponseAsync($"<:unavailable:828216690525339658> You cannot execute this command:\n{checkResult.Reason}");
                throw new Exception($"Checks failed for command '{command.Class.Name}': {checkResult.Reason}.");
            }
        }
    }
}