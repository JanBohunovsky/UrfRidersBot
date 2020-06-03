using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace UrfRiders.Modules.Clash
{
    [Name("Clash")]
    [Group("clash")]
    [RequireContext(ContextType.Guild)]
    //[RequireLevel(PermissionLevel.Admin)]
    public class ClashModule : BaseModule
    {
        public ClashService Service { get; set; }
        public ILogger<ClashModule> Logger { get; set; }

        private EmbedBuilder BaseEmbed => new EmbedBuilder().WithColor(Color.Blue);


    }
}