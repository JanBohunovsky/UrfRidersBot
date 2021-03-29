using System.Collections.Generic;
using UrfRidersBot.Core.Commands.Built;

namespace UrfRidersBot.Core.Interfaces
{
    public interface ICommandManager
    {
        IEnumerable<SlashCommand> BuildCommands();
    }
}