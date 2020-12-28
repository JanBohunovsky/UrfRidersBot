using System.Threading.Tasks;
using Discord;

namespace UrfRidersBot.Library
{
    public interface IHelpService
    {
        ValueTask<Embed> GetCommandDetails(UrfRidersContext context, string commandName);
        ValueTask<Embed> GetAllCommands(UrfRidersContext context);
    }
}