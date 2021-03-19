using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using UrfRidersBot.Infrastructure;

namespace UrfRidersBot.Commands.Modules
{
    public class TestModule : ModuleBase<UrfRidersCommandContext>
    {
        [Command("test")]
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
        public async Task Test(DiscordUser value)
        {
            await Context.RespondAsync(value.Mention);
        }

        [Command("member")]
        public async Task Test(DiscordMember value)
        {
            await Context.RespondAsync(value.Mention);
        }

        [Command("role")]
        public async Task Test(DiscordRole value)
        {
            await Context.RespondAsync(value.Mention);
        }

        [Command("channel")]
        public async Task Test(DiscordChannel value)
        {
            await Context.RespondAsync(value.Mention);
        }

        [Command("guild")]
        public async Task Test(DiscordGuild value)
        {
            await Context.RespondAsync(value.Name);
        }

        [Command("message")]
        public async Task Test(DiscordMessage value)
        {
            await Context.RespondAsync(value.JumpLink.ToString());
        }

        [Command("emoji")]
        public async Task Test(DiscordEmoji value)
        {
            await Context.RespondAsync(value.ToString());
        }

        [Command("color")]
        public async Task Test(DiscordColor value)
        {
            await Context.RespondAsync(value.ToString());
        }
    }
}