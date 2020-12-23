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
        public async Task<RuntimeResult> Success()
        {
            return CommandResult.FromSuccess("Yay, everything went well!");
        }

        [Command("error")]
        public async Task<RuntimeResult> Error()
        {
            return CommandResult.FromError("Something went wrong.");
        }

        [Command("info")]
        public async Task<RuntimeResult> Information()
        {
            return CommandResult.FromInformation(null, "Hello world!");
        }

        [Command("everyone")]
        [RequireGuildRank(GuildRank.Everyone)]
        public async Task<RuntimeResult> Everyone()
        {
            return CommandResult.FromSuccess("Yay, everyone can use this command!");
        }

        [Command("member")]
        [RequireGuildRank(GuildRank.Member)]
        public async Task<RuntimeResult> Member()
        {
            return CommandResult.FromSuccess("Yay, you're a verified user on this server!");
        }

        [Command("moderator")]
        [RequireGuildRank(GuildRank.Moderator)]
        public async Task<RuntimeResult> Moderator()
        {
            return CommandResult.FromSuccess("You're a moderator! Congrats!");
        }

        [Command("admin")]
        [RequireGuildRank(GuildRank.Admin)]
        public async Task<RuntimeResult> Admin()
        {
            return CommandResult.FromSuccess("Damn look at this admin right here!");
        }

        [Command("owner")]
        [RequireGuildRank(GuildRank.Owner)]
        public async Task<RuntimeResult> Owner()
        {
            return CommandResult.FromSuccess("The king of the castle! eh.. I mean the owner of this server!");
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