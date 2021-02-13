using DSharpPlus.CommandsNext;

namespace UrfRidersBot.Discord.Commands.Modules
{
    public class UrfRidersCommandModule : BaseCommandModule
    {
        public IEmbedService EmbedService { get; set; } = null!;
    }
}