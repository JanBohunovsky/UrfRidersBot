using Discord.Commands;
using UrfRidersBot.Library;

namespace UrfRidersBot.ConsoleUI.Modules
{
    public class BaseModule : ModuleBase<UrfRidersContext>
    {
        public IEmbedService Embed { get; set; } = null!;
    }
}