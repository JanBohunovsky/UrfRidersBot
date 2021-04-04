using System.Collections.Generic;
using UrfRidersBot.Core.Commands.Built;

namespace UrfRidersBot.Core.Commands.Services
{
    public interface ICommandManager
    {
        IEnumerable<SlashCommand> BuildCommands();
    }
}