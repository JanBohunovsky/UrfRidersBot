using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace UrfRidersBot
{
    [RequireGuildRank(GuildRank.Admin)]
    public class SystemModule : UrfRidersCommandModule
    {
        [Command("shutdown")]
        [Description("Stops the application the bot is running on.")]
        public async Task Shutdown(CommandContext ctx)
        {
            var embed = EmbedService
                .CreateBotInfo()
                .WithDescription("Shutting down...")
                .Build();
            
            await ctx.RespondAsync(embed);
            
            Program.Shutdown();
        }

        [Command("restart")]
        [Description(
            "Soft-restarts the application the bot is running on.\n" +
            "This won't actually restart the application, it will only stop the application [host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host) and do the start-up sequence once again.\n" + 
            "This process should be equivalent to an application restart, including getting rid of memory leaks if there are any (hopefully).")]
        public async Task Restart(CommandContext ctx)
        {
            var embed = EmbedService
                .CreateBotInfo()
                .WithDescription("Restarting...")
                .Build();
            
            await ctx.RespondAsync(embed);
            
            Program.Restart();
        }
    }
}