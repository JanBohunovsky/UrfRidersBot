using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands.Built;

namespace UrfRidersBot.Core.Commands.Services
{
    public interface ICommandHandler
    {
        void AddCommands(IEnumerable<SlashCommandDefinition> commands);
        
        Task HandleAsync(DiscordInteraction interaction);
    }
}