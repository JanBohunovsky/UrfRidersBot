using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Library;

namespace UrfRidersBot.ConsoleUI.Modules
{
    [Group("test")]
    [RequireOwner]
    [Name("Test")]
    [Summary("Debug commands to test bot's functionality.")]
    public class TestModule : BaseModule
    {
        public UrfRidersDbContext DbContext { get; set; } = null!;
        public ILogger<TestModule> Logger { get; set; } = null!;

        [Command]
        [Name("Test")]
        [Summary("This will do a simple test.")]
        public async Task Test()
        {
            await ReplyAsync(embed: Embed.Basic(title: "Test completed").Build());
        }
        
        [Command("exception")]
        [Name("Exception test")]
        [Summary("This command will throw an exception.")]
        public Task Exception()
        {
            throw new NotImplementedException();
        }

        [Command("success")]
        [Name("Success test")]
        [Summary("Responds with a success message.")]
        public async Task Success()
        {
            await ReplyAsync(embed: Embed.Success("Yay, everything went well!").Build());
        }

        [Command("error")]
        [Name("Error test")]
        [Summary("Responds with an error message.")]
        public async Task Error()
        {
            await ReplyAsync(embed: Embed.Error("Something went wrong.").Build());
        }

        [Command("info")]
        [Name("Information test")]
        [Summary("Responds with basic message.")]
        public async Task Information()
        {
            await ReplyAsync(embed: Embed.Basic(title: "Hello world!").Build());
        }

        [Command("database")]
        [Alias("db", "prefix")]
        [Name("Database test")]
        [Summary("Gets or sets this server's custom prefix.")]
        public async Task Database(string? newPrefix = null)
        {
            var settings = await DbContext.GuildSettings.FindOrCreateAsync(Context.Guild.Id);

            var embed = Embed.Basic(title: "Custom prefix");
            if (newPrefix == null)
            {
                embed.WithDescription(settings.CustomPrefix == null
                    ? "This server doesn't have custom prefix set"
                    : $"Current custom prefix: `{settings.CustomPrefix}`");
            }
            else
            {
                if (newPrefix.ToLower() == "reset")
                    newPrefix = null;
                
                embed
                    .WithDescription("Custom prefix on this server has been updated.")
                    .AddField("Before", settings.CustomPrefix.ToCode() ?? "*None*", true)
                    .AddField("After", newPrefix.ToCode() ?? "*None*", true);
                
                settings.CustomPrefix = newPrefix;
            }
            
            await DbContext.SaveChangesAsync();

            await ReplyAsync(embed: embed.Build());
        }
    }
}