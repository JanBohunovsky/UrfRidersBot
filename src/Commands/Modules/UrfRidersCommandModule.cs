using Discord.Commands;

namespace UrfRidersBot
{
    public class UrfRidersCommandModule : ModuleBase<UrfRidersContext>
    {
        public EmbedService EmbedService { get; set; } = null!;
    }
}