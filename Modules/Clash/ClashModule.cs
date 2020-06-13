using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using UrfRiders.Util;

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

        [Command("init")]
        [RequireLevel(PermissionLevel.Admin)]
        public async Task InitializeClashChannel()
        {
            if (!Settings.ClashChannel.HasValue)
                Settings.ClashChannel = Context.Channel.Id;
            if (Settings.ClashChannel != Context.Channel.Id)
                return;

            // Create "Upcoming Tournaments" message and pin it
            var embedBuilder = Service.CreateTournamentListEmbed(ClashService.TitleUpcomingTournaments);
            if (embedBuilder.Fields.Count == 0)
                embedBuilder.WithDescription(ClashService.DescriptionNoTournaments);

            var upcomingMessage = await ReplyAsync(embed: embedBuilder.Build());
            await upcomingMessage.PinAsync();

            // Update settings
            Settings.ClashPinnedMessage = upcomingMessage.Id;
            Settings.SeenTournaments = Service.Tournaments.Select(t => t.TournamentId).ToList();
            Settings.Save();

            // Update channel topic
            await Service.UpdateChannelTopic(Context.Channel as SocketTextChannel);
        }

        [Command("reset")]
        [RequireLevel(PermissionLevel.Admin)]
        public async Task ResetClashChannel()
        {
            var channel = Context.Channel as ITextChannel;
            if (Settings.ClashChannel.HasValue && Settings.ClashChannel.Value != Context.Channel.Id)
                channel = Context.Client.GetChannel(Settings.ClashChannel.Value) as ITextChannel;

            if (channel == null)
            {
                Logger.LogWarning("channel is null");
                return;
            }

            // Delete pinned message
            if (Settings.ClashPinnedMessage.HasValue)
            {
                var message = await channel.GetMessageAsync(Settings.ClashPinnedMessage.Value);
                await message.DeleteAsync();
            }

            // Clear settings
            Settings.ClashChannel = null;
            Settings.ClashPinnedMessage = null;
            Settings.SeenTournaments.Clear();
            Settings.Save();

            await ReplyAsync(embed: new EmbedBuilder().WithSuccess("Clash module has been reset.").Build());

            // Clear channel topic
            //await channel.ModifyAsync(c => c.Topic = "");
        }

        [Command("tournaments")]
        [Alias("upcoming")]
        public async Task GetTournaments()
        {
            if (Settings.ClashChannel != Context.Channel.Id)
                return;

            var embedBuilder = Service.CreateTournamentListEmbed(ClashService.TitleUpcomingTournaments);
            if (embedBuilder.Fields.Count == 0)
                embedBuilder.WithDescription(ClashService.DescriptionNoTournaments);

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("topic")]
        [RequireLevel(PermissionLevel.Moderator)]
        public async Task UpdateChannelTopic()
        {
            if (Settings.ClashChannel != Context.Channel.Id)
                return;

            await Service.UpdateChannelTopic(Context.Channel as SocketTextChannel);
        }
    }
}