using System;
using System.Threading.Tasks;
using Discord.Commands;
using UrfRidersBot.Interactive;

namespace UrfRidersBot
{
    [Group("test")]
    [RequireOwner]
    [Name("Test")]
    [Summary("Debug commands to test bot's functionality.")]
    public class TestModule : UrfRidersCommandModule
    {
        public UrfRidersDbContext DbContext { get; set; } = null!;
        public InteractiveService InteractiveService { get; set; } = null!;

        [Command]
        [Priority(0)]
        [Name("Test")]
        [Summary("This will do a simple test.")]
        public async Task Test()
        {
            await ReplyAsync(embed: EmbedService.CreateBasic(title: "Test completed").Build());
        }
        
        [Command("exception")]
        [Priority(1)]
        [Name("Exception test")]
        [Summary("This command will throw an exception.")]
        public Task Exception()
        {
            throw new NotImplementedException();
        }

        [Command("success")]
        [Priority(1)]
        [Name("Success test")]
        [Summary("Responds with a success message.")]
        public async Task Success()
        {
            await ReplyAsync(embed: EmbedService.CreateSuccess("Yay, everything went well!").Build());
        }

        [Command("error")]
        [Priority(1)]
        [Name("Error test")]
        [Summary("Responds with an error message.")]
        public async Task Error()
        {
            await ReplyAsync(embed: EmbedService.CreateError("Something went wrong.").Build());
        }

        [Command("info")]
        [Priority(1)]
        [Name("Information test")]
        [Summary("Responds with basic message.")]
        public async Task Information()
        {
            await ReplyAsync(embed: EmbedService.CreateBasic(title: "Hello world!").Build());
        }

        [Command("reactionTracker add")]
        [Priority(1)]
        [Name("Add reaction tracker")]
        [Summary("You will get notified whenever a user adds/removes a reaction to/from target message.")]
        public async Task AddReactionTracker(ulong messageId)
        {
            if (InteractiveService.HasReactionHandler(messageId))
            {
                var embed = EmbedService.CreateError("This message already has some kind of reaction handler.").Build();
                await ReplyAsync(embed: embed);
            }
            else
            {
                // Create new reaction handler and create persistent data for it.
                await InteractiveService.AddReactionHandlerAsync<ReactionTrackerHandler>(messageId);
                await DbContext.ReactionTrackerData.AddAsync(new ReactionTrackerData(messageId, Context.User.Id));
                await DbContext.SaveChangesAsync();

                var embed = EmbedService
                    .CreateSuccess($"Message is now being tracked for reactions.")
                    .Build();

                await ReplyAsync(embed: embed);
            }
            
        }

        [Command("reactionTracker remove")]
        [Priority(1)]
        [Name("Remove reaction tracker")]
        public async Task RemoveReactionTracker(ulong messageId)
        {
            if (!InteractiveService.HasReactionHandler<ReactionTrackerHandler>(messageId))
            {
                var embed = EmbedService.CreateError("This message doesn't have reaction tracker.").Build();
                await ReplyAsync(embed: embed);
            }
            else
            {
                // Remove the reaction handler and its persistent data.
                await InteractiveService.RemoveReactionHandlerAsync(messageId);
                var data = await DbContext.ReactionTrackerData.FindAsync(messageId);
                DbContext.ReactionTrackerData.Remove(data);
                await DbContext.SaveChangesAsync();
                
                var embed = EmbedService
                    .CreateSuccess($"Message is no longer being tracked for reactions.")
                    .Build();

                await ReplyAsync(embed: embed);
            }
            
        }
    }
}