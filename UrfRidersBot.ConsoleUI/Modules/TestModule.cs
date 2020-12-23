using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Library;
using UrfRidersBot.Library.Preconditions;

namespace UrfRidersBot.ConsoleUI.Modules
{
    [Group("test")]
    public class TestModule : BaseModule
    {
        public UrfRidersContext DbContext { get; set; } = null!;
        public ILogger<TestModule> Logger { get; set; } = null!;
        
        [Command("exception")]
        public Task Exception()
        {
            throw new NotImplementedException();
        }

        [Command("success")]
        public async Task Success()
        {
            await ReplyAsync(embed: Embed.Success("Yay, everything went well!").Build());
        }

        [Command("error")]
        public async Task Error()
        {
            await ReplyAsync(embed: Embed.Error("Something went wrong.").Build());
        }

        [Command("info")]
        public async Task Information()
        {
            await ReplyAsync(embed: Embed.Basic(title: "Hello world!").Build());
        }

        [Command("everyone")]
        [RequireGuildRank(GuildRank.Everyone)]
        public async Task Everyone()
        {
            await ReplyAsync(embed: Embed.Success("Yay, everyone can use this command!").Build());
        }

        [Command("member")]
        [RequireGuildRank(GuildRank.Member)]
        public async Task Member()
        {
            await ReplyAsync(embed: Embed.Success("Yay, you're a verified user on this server!").Build());
        }

        [Command("moderator")]
        [RequireGuildRank(GuildRank.Moderator)]
        public async Task Moderator()
        {
            await ReplyAsync(embed: Embed.Success("You're a moderator! Congrats!").Build());
        }

        [Command("admin")]
        [RequireGuildRank(GuildRank.Admin)]
        public async Task Admin()
        {
            await ReplyAsync(embed: Embed.Success("Damn look at this admin right here!").Build());
        }

        [Command("owner")]
        [RequireGuildRank(GuildRank.Owner)]
        public async Task Owner()
        {
            await ReplyAsync(embed: Embed.Success("The king of the castle! eh.. I mean the owner of this server!").Build());
        }

        [Command("database")]
        [Alias("db")]
        public async Task Database(string? value = null)
        {
            Logger.LogInformation("Database access test");
            var guildData = await DbContext.GuildData.FindOrCreateAsync(Context.Guild.Id);

            var embed = Embed.Basic(title: "Database test");
            if (value != null)
            {
                embed
                    .AddField("Before", guildData.RandomValue ?? "*<null>*", true)
                    .AddField("After", value, true);
                
                guildData.RandomValue = value;
            }
            else
            {
                embed.AddField("Value", guildData.RandomValue ?? "*<null>*");
            }
            
            await DbContext.SaveChangesAsync();

            await ReplyAsync(embed: embed.Build());
        }
    }
}