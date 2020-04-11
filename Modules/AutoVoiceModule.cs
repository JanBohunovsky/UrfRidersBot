using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text;
using System.Threading.Tasks;
using UrfRiders.Attributes.Preconditions;
using UrfRiders.Data;
using UrfRiders.Services;

namespace UrfRiders.Modules
{
    [Name("Auto Voice")]
    [Group("autovoice")]
    [RequireContext(ContextType.Guild)]
    [RequireLevel(PermissionLevel.Admin)]
    public class AutoVoiceModule : BaseModule
    {
        public AutoVoiceService Service { get; set; }

        private EmbedBuilder BaseEmbed => new EmbedBuilder().WithColor(Program.Color).WithTitle("Auto Voice Module");

        [Command]
        public async Task ModuleHelp()
        {
            var enabled = Settings.AutoVoiceChannels.Count > 0;
            var changeText = enabled ? "Disable" : "Enable";
            var embed = BaseEmbed
                .WithDescription($"{(enabled ? "✅" : "❌")} Module is {(enabled ? "enabled" : "disabled")} for this server.")
                .AddField($"{changeText} Module", $"`{Settings.Prefix}autovoice {changeText.ToLower()}`")
                .AddField("List all voice channels", $"`{Settings.Prefix}autovoice list`")
                .AddField("Show Activities", $"`{Settings.Prefix}autovoice activity`")
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("activity")]
        [Alias("activities")]
        public async Task Activities()
        {
            // Cast into guild user (should never fail because of RequireContext)
            if (!(Context.User is SocketGuildUser user))
                return;

            var embedBuilder = BaseEmbed.WithDescription("Activities");
            if (user.VoiceChannel == null)
            {
                embedBuilder
                    .AddField("Your activity", user.Activity?.ToString() ?? "None")
                    .AddField("Note", "Use this command while in a voice channel to get activities from all users in that channel.");
            }
            else
            {
                foreach (var voiceUser in user.VoiceChannel.Users)
                {
                    if (voiceUser.Activity != null)
                        embedBuilder.AddField(voiceUser.Username, FormatActivity(voiceUser.Activity));
                }
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("list")]
        [Alias("channels")]
        public async Task List()
        {
            var sbChannels = new StringBuilder();
            var sbIds = new StringBuilder();
            foreach (var channelId in Settings.AutoVoiceChannels)
            {
                var channel = Context.Guild.GetVoiceChannel(channelId);
                var userCount = channel.Users.Count;
                sbChannels.AppendLine($"`{channel.Name}` - {userCount} user{(userCount == 1 ? "" : "s")}");
                sbIds.AppendLine(channel.Id.ToString().ToCode());
            }

            var embed = BaseEmbed
                .AddField("Voice Channel", sbChannels.ToString(), true)
                .AddField("ID", sbIds.ToString(), true)
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("enable")]
        public async Task Enable()
        {
            var success = await Service.Enable(Context.Guild);
            var embedBuilder = BaseEmbed;

            if (success)
            {
                embedBuilder
                    .WithSuccess(
                        $"A new voice channel called `{AutoVoiceService.NameDefault}` has been created, feel free to move it around.")
                    .AddField("How it works",
                        "Whenever there are no empty voice channels, I will create a new one.\n" +
                        "When all users leave my voice channel, it will be automatically deleted (except for the last one).")
                    .AddField("Disable", $"To disable this module type `{Settings.Prefix}autovoice disable`");
            }
            else
            {
                embedBuilder.WithError("Module is already enabled in this server.");
            }
            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("disable")]
        public async Task Disable()
        {
            var success = Service.Disable(Context.Guild);
            var embedBuilder = BaseEmbed;

            if (success)
            {
                embedBuilder
                    .WithSuccess("Module disabled.\nAll of my voice channels have been removed.")
                    .AddField("Enable", $"To enable this module again, type `{Settings.Prefix}autovoice enable`");
            }
            else
            {
                embedBuilder.WithError("Module is not enabled in this server.");
            }
            await ReplyAsync(embed: embedBuilder.Build());
        }

        private string FormatActivity(IActivity activity)
        {
            var icon = activity.Type switch
            {
                ActivityType.Listening => "🎵",
                ActivityType.Playing => "🎮",
                ActivityType.Watching => "👀",
                ActivityType.Streaming => "📺",
                (ActivityType)4 => "🔧", // Custom Status
                _ => "❔"
            };

            return $"\\{icon} {activity.Name}";
        }
    }
}