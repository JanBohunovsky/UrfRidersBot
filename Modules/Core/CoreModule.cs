using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using UrfRiders.Util;

namespace UrfRiders.Modules.Core
{
    [Name("Core")]
    public class CoreModule : BaseModule
    {
        public CommandService CommandService { get; set; }
        public ILogger<CoreModule> Logger { get; set; }

        [Command("info")]
        [Alias("about", "version")]
        public async Task Info()
        {
#if DEBUG
            var mode = "Development";
#else
            var mode = "Release";
#endif
            var appInfo = await Context.Client.GetApplicationInfoAsync();
            var modules = CommandService.Modules
                .Where(m => m.Name != "BaseModule")
                .Select(m => m.Name.Replace("Module", ""))
                .OrderBy(s => s);

            var embed = new EmbedBuilder()
                .WithColor(Program.Color)
                .WithTitle("Official UrfRiders Bot")
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .AddField("Owner", appInfo.Owner.Mention, true)
                .AddField("Mode", mode, true)
                .AddField("Version", Program.Version, true)
                .AddField("Modules", string.Join("\n", modules))
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("server")]
        public async Task Server()
        {
            var embed = new EmbedBuilder()
                .WithColor(Program.Color)
                .WithTitle(Context.Guild.Name)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("ID", Context.Guild.Id.ToString().ToCode())
                .AddField("Owner", Context.Guild.Owner.Mention)
                .AddField("Members", Context.Guild.MemberCount)
                .AddField("Created", Context.Guild.CreatedAt.ToString("dd MMMM yyyy, hh:mm:ss tt"))
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("help")]
        [Alias("techsupport")]
        public async Task Help(IUser targetUser = null)
        {
            var appInfo = await Context.Client.GetApplicationInfoAsync();
            var embed = new EmbedBuilder()
                .WithColor(Program.Color)
                .WithThumbnailUrl("https://u.cubeupload.com/Bohush/tester.png")
                .WithTitle("Tech Support")
                .WithDescription("Feel free to contact our tech support:")
                .AddField("Member", appInfo.Owner.Mention, true)
                .AddField("Availability", "10 AM - 6 PM CEST", true)
                .WithFooter("Member availability may not be guaranteed.")
                .Build();

            await ReplyAsync(targetUser?.Mention, embed: embed);
        }

        [Command("roles")]
        [RequireLevel(PermissionLevel.Admin)]
        public async Task Roles()
        {
            var embed = new EmbedBuilder()
                .WithColor(Program.Color)
                .WithTitle("Server roles")
                .AddField("Admin", Settings.AdminRole.HasValue ? Context.Guild.GetRole(Settings.AdminRole.Value).Mention : "None")
                .AddField("Moderator", Settings.ModeratorRole.HasValue ? Context.Guild.GetRole(Settings.ModeratorRole.Value).Mention : "None")
                .AddField("Member", Settings.MemberRole.HasValue ? Context.Guild.GetRole(Settings.MemberRole.Value).Mention : "None")
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("test")]
        [RequireOwner]
        public async Task Test(string emoji)
        {
            await ReplyAsync(embed: new EmbedBuilder().WithColor(Program.Color).WithDescription("Nothing is being tested right now.").Build());
        }

        [Command("embed")]
        [RequireLevel(PermissionLevel.Moderator)]
        public async Task CreateEmbed(SocketTextChannel channel, [Remainder] string json)
        {
            var embedBuilder = new EmbedBuilder();

            try
            {
                var embedJson = JObject.Parse(json);

                if (embedJson.ContainsKey("title"))
                    embedBuilder.Title = (string)embedJson["title"];

                if (embedJson.ContainsKey("description"))
                    embedBuilder.Description = (string)embedJson["description"];

                if (embedJson.ContainsKey("url"))
                    embedBuilder.Url = (string)embedJson["url"];

                if (embedJson.ContainsKey("color"))
                    embedBuilder.Color = new Color(embedJson["color"].Value<uint>());

                if (embedJson.ContainsKey("timestamp"))
                    embedBuilder.Timestamp = DateTimeOffset.Parse((string)embedJson["timestamp"]);

                if (embedJson.ContainsKey("footer"))
                {
                    var footer = embedJson["footer"].Value<JObject>();
                    embedBuilder.Footer = new EmbedFooterBuilder();

                    if (footer.ContainsKey("icon_url"))
                        embedBuilder.Footer.IconUrl = (string)footer["icon_url"];

                    if (footer.ContainsKey("text"))
                        embedBuilder.Footer.Text = (string)footer["text"];
                }

                if (embedJson.ContainsKey("thumbnail"))
                {
                    embedBuilder.ThumbnailUrl = (string)embedJson["thumbnail"]["url"];
                }

                if (embedJson.ContainsKey("image"))
                {
                    embedBuilder.ImageUrl = (string)embedJson["image"]["url"];
                }

                if (embedJson.ContainsKey("author"))
                {
                    var author = embedJson["author"].Value<JObject>();
                    embedBuilder.Author = new EmbedAuthorBuilder();

                    if (author.ContainsKey("name"))
                        embedBuilder.Author.Name = (string)author["name"];

                    if (author.ContainsKey("url"))
                        embedBuilder.Author.Url = (string)author["url"];

                    if (author.ContainsKey("icon_url"))
                        embedBuilder.Author.IconUrl = (string)author["icon_url"];
                }

                if (embedJson.ContainsKey("fields"))
                {
                    foreach (var field in embedJson["fields"].Values<JObject>())
                    {
                        bool inline = false;
                        if (field.ContainsKey("inline"))
                            inline = field["inline"].Value<bool>();

                        embedBuilder.AddField((string)field["name"], (string)field["value"], inline);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogInformation($"Failed to parse embed: {e.Message}");
                await ReplyAsync(embed: new EmbedBuilder().WithError("Invalid json or embed format.\n[Template](https://leovoel.github.io/embed-visualizer/)").Build());
                return;
            }

            await channel.SendMessageAsync(embed: embedBuilder.Build());
            await ReplyAsync(embed: new EmbedBuilder().WithSuccess("Embed created.").Build());
        }
    }
}