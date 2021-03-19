using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;
using UrfRidersBot.Core;
using UrfRidersBot.Infrastructure.Commands;

namespace UrfRidersBot.Commands
{
    [Group("test")]
    public class TestModule : ModuleBase<UrfRidersCommandContext>
    {
        [Command]
        [Description("Just simple test command.")]
        public async Task Test()
        {
            var embed = EmbedHelper
                .CreateSuccess("Test successful! :+1:")
                .AddField("Prefix", Context.Prefix, true)
                .AddField("Member", Context.Member?.Mention ?? "null", true);
            
            await Context.RespondAsync(embed);
        }

        [Command("user")]
        public async Task TestUser(DiscordUser user)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = user.Username,
                Timestamp = user.CreationTimestamp,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = user.GetAvatarUrl(ImageFormat.Png, 128),
                    Width = 128,
                    Height = 128
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Joined discord:"
                }
            };
            
            await Context.RespondAsync(embed);
        }

        [Command("member")]
        public async Task TestMember(DiscordMember member)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = member.Username,
                Timestamp = member.CreationTimestamp,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = member.GetAvatarUrl(ImageFormat.Png, 128),
                    Width = 128,
                    Height = 128
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Joined discord:"
                }
            };

            if (member.Color.Value != DiscordColor.None.Value)
            {
                embed.Color = member.Color;
            }

            embed.AddField("Nickname", member.Nickname ?? "*none*", true);
            embed.AddField("Language", member.Locale ?? "*unknown*", true);
            embed.AddField("Server join date", member.JoinedAt.ToString(), true);
            embed.AddField("Roles", string.Join('\n', member.Roles.Select(r => r.Mention)), true);

            await Context.RespondAsync(embed);
        }

        [Command("role")]
        public async Task TestRole(DiscordRole role)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = role.Name,
                Timestamp = role.CreationTimestamp,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Created:"
                }
            };

            if (role.Color.Value != DiscordColor.None.Value)
            {
                embed.Color = role.Color;
            }
            
            await Context.RespondAsync(embed);
        }

        [Command("channel")]
        public async Task TestChannel(DiscordChannel channel)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Theme,
                Title = channel.Name,
                Timestamp = channel.CreationTimestamp,
                Url = $"https://discord.com/channels/{channel.GuildId}/{channel.Id}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Created:"
                }
            };

            embed.AddField("Type", channel.Type.ToString(), true);
            embed.AddField("Position", channel.Position.ToString(), true);
            
            await Context.RespondAsync(embed);
        }

        [Command("guild")]
        public async Task TestGuild(DiscordGuild guild)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Theme,
                Title = guild.Name,
                Timestamp = guild.CreationTimestamp,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = guild.IconUrl,
                    Width = 128,
                    Height = 128
                },
                ImageUrl = guild.BannerUrl,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Joined discord:"
                }
            };

            embed.AddField("Channels", guild.Channels.Count.ToString(), true);
            embed.AddField("Members", guild.Members.Count.ToString(), true);
            embed.AddField("Roles", guild.Roles.Count.ToString(), true);
            
            await Context.RespondAsync(embed);
        }

        [Command("message")]
        public async Task TestMessage(DiscordMessage message)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Theme,
                Timestamp = message.CreationTimestamp,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = message.Author.AvatarUrl,
                    Name = message.Author.Username,
                    Url = message.JumpLink.ToString()
                },
                Description = message.Content,
            };
            
            await Context.RespondAsync(embed);
        }

        [Command("emoji")]
        public async Task TestEmoji(DiscordEmoji emoji)
        {
            await Context.Message.CreateReactionAsync(emoji);
        }

        [Command("color")]
        public async Task TestColor(DiscordColor color)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = color,
                Description = Markdown.Code(color.ToString())
            };
                
            await Context.RespondAsync(embed);
        }
    }
}