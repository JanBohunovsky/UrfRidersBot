using System.Threading.Tasks;
using Discord.Commands;
using UrfRidersBot.Library;

namespace UrfRidersBot.ConsoleUI.Modules
{
    [RequireGuildRank(GuildRank.Admin)]
    [Name("System")]
    public class SystemModule : BaseModule
    {
        [Command("shutdown")]
        [Name("Shutdown")]
        [Summary("Shutdowns the bot.")]
        public async Task Shutdown()
        {
            var embed = EmbedService.CreateBasic("Shutting down...", "System").Build();
            await ReplyAsync(embed: embed);
            
            Program.Shutdown();
        }

        [Command("restart")]
        [Name("Restart")]
        [Summary("Soft restarts the bot. This will only stop the host and rebuild all services. This will **not** stop the application.")]
        public async Task Restart()
        {
            var embed = EmbedService.CreateBasic("Restarting...", "System").Build();
            await ReplyAsync(embed: embed);
            
            Program.Restart();
        }
    }
}