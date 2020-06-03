using System.Text;
using System.Threading.Tasks;
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

        [Command("tournaments")]
        [Alias("upcoming")]
        public async Task GetTournaments()
        {
            if (Settings.ClashChannel != Context.Channel.Id)
                return;

            var embedBuilder = BaseEmbed.WithTitle("Upcoming Tournaments");

            var tournaments = await Service.GetTournaments();
            if (tournaments.Length == 0)
            {
                await ReplyAsync(embed: embedBuilder.WithDescription("No upcoming tournaments.").Build());
                return;
            }

            var description = new StringBuilder();
            foreach (var tournament in tournaments)
            {
                description.AppendLine($"**{tournament.FormattedName}**");
                description.AppendLine(tournament.FormattedTime);
                description.AppendLine();
            }

            embedBuilder.WithDescription(description.ToString());
            embedBuilder.WithThumbnailUrl("https://cdn.discordapp.com/attachments/717788228899307551/717788431006302218/Clash_Crest_icon.webp");
            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}