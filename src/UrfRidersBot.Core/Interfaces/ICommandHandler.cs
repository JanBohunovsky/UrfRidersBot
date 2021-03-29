using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands.Built;

namespace UrfRidersBot.Core.Interfaces
{
    public interface ICommandHandler
    {
        void SetCommands(IEnumerable<SlashCommand> commands);
        
        Task HandleAsync(DiscordInteraction interaction);
    }
}