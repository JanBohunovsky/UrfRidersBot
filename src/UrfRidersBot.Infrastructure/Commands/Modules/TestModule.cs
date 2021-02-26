using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using UrfRidersBot.Core;

namespace UrfRidersBot.Infrastructure.Commands.Modules
{
    [Group("test")]
    [RequireOwner]
    [Description("Debug commands to test bot's functionality.")]
    public class TestModule : BaseCommandModule
    {
        [Command("exception")]
        [Description("This command will throw an exception.")]
        public async Task Exception(CommandContext ctx)
        {
            DiscordEmbedBuilder embed = null!;
            
            await ctx.RespondAsync($"Test: {embed.Title}");
        }

        [Command("success")]
        [Description("Responds with a success message.")]
        public async Task Success(CommandContext ctx)
        {
            await ctx.RespondAsync(EmbedHelper.CreateSuccess("Yay, everything went well!").Build());
        }

        [Command("error")]
        [Description("Responds with an error message.")]
        public async Task Error(CommandContext ctx)
        {
            await ctx.RespondAsync(EmbedHelper.CreateError("Something went wrong.").Build());
        }

        [Command("info")]
        [Description("Responds with basic message.")]
        public async Task Information(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Cyan,
                Title = "Hello world!",
                Description = ":wave:",
            };
            
            await ctx.RespondAsync(embed.Build());
        }
    }
}