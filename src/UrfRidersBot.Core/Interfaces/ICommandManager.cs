using System.Collections.Generic;
using UrfRidersBot.Core.Commands.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface ICommandManager
    {
        IEnumerable<SlashCommand> BuildCommands();
    }
}