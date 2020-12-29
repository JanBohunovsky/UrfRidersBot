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
            await ReplyAsync(embed: EmbedService.CreateBasic(title: "Test completed").Build());
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
            await ReplyAsync(embed: EmbedService.CreateSuccess("Yay, everything went well!").Build());
        }

        [Command("error")]
        [Name("Error test")]
        [Summary("Responds with an error message.")]
        public async Task Error()
        {
            await ReplyAsync(embed: EmbedService.CreateError("Something went wrong.").Build());
        }

        [Command("info")]
        [Name("Information test")]
        [Summary("Responds with basic message.")]
        public async Task Information()
        {
            await ReplyAsync(embed: EmbedService.CreateBasic(title: "Hello world!").Build());
        }
    }
}