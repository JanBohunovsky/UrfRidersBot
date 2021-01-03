using System.Threading.Tasks;
using Discord.Commands;

namespace UrfRidersBot
{
    [RequireGuildRank(GuildRank.Admin)]
    [Name("System")]
    public class SystemModule : UrfRidersCommandModule
    {
        [Command("shutdown")]
        [Name("Shutdown")]
        [Summary("Stops the application the bot is running on.")]
        public async Task Shutdown()
        {
            var embed = EmbedService
                .CreateBotInfo()
                .WithDescription("Shutting down...")
                .Build();
            
            await ReplyAsync(embed: embed);
            
            Program.Shutdown();
        }

        [Command("restart")]
        [Name("Restart")]
        [Summary("Soft-restarts the application the bot is running on.\n" +
                 "This won't actually restart the application, it will only stop the application [host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host) and do the start-up sequence once again.\n" + 
                 "This process should be equivalent to an application restart, including getting rid of memory leaks if there are any (hopefully).")]
        public async Task Restart()
        {
            var embed = EmbedService
                .CreateBotInfo()
                .WithDescription("Restarting...")
                .Build();
            
            await ReplyAsync(embed: embed);
            
            Program.Restart();
        }
    }
}