using Discord.Commands;
using UrfRidersBot.Library;

namespace UrfRidersBot.ConsoleUI.Modules
{
    public class BaseModule : ModuleBase<SocketCommandContext>
    {
        public IEmbedService Embed { get; set; } = null!;
    }
}