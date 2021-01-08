using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Interactive;

namespace UrfRidersBot
{
    [Group("test")]
    [RequireOwner]
    [Description("Debug commands to test bot's functionality.")]
    public class TestModule : UrfRidersCommandModule
    {
        public ILogger<TestModule> Logger { get; set; } = null!;

        [Command("exception")]
        [Description("This command will throw an exception.")]
        public Task Exception(CommandContext ctx)
        {
            throw new NotImplementedException();
        }

        [Command("success")]
        [Description("Responds with a success message.")]
        public async Task Success(CommandContext ctx)
        {
            await ctx.RespondAsync(EmbedService.CreateSuccess("Yay, everything went well!").Build());
        }

        [Command("error")]
        [Description("Responds with an error message.")]
        public async Task Error(CommandContext ctx)
        {
            await ctx.RespondAsync(EmbedService.CreateError("Something went wrong.").Build());
        }

        [Command("info")]
        [Description("Responds with basic message.")]
        public async Task Information(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Cyan,
                Title = "Hello world!",
            };
            
            await ctx.RespondAsync(":wave:", embed.Build());
        }

        [Group("reactionTracker")]
        [Description("Attach to a message to get notified whenever a someone adds or removes a reaction.")]
        public class ReactionTrackerSubmodule : UrfRidersCommandModule
        {
            public IDbContextFactory<UrfRidersDbContext> DbContextFactory { get; set; } = null!;
            public InteractiveService InteractiveService { get; set; } = null!;
            
            [Command("add")]
            [Description("Add a reaction tracker to specified message.")]
            public async Task Add(CommandContext ctx, [Description("Message to attach reaction tracker on.")] DiscordMessage message)
            {
                if (InteractiveService.HasReactionHandler(message.Id))
                {
                    var embed = EmbedService.CreateError("This message already has some kind of reaction handler.").Build();
                    await ctx.RespondAsync(embed);
                }
                else
                {
                    // Create new reaction handler and create persistent data for it.
                    // TODO: Make this into a service method. I know this is a test command but something like this will be used in the future.
                    await using (var dbContext = DbContextFactory.CreateDbContext())
                    {
                        await InteractiveService.AddReactionHandlerAsync<ReactionTrackerHandler>(message.Id);
                        await dbContext.ReactionTrackerData.AddAsync(new ReactionTrackerData(message.Id, ctx.User.Id));
                        await dbContext.SaveChangesAsync();
                    }

                    var embed = EmbedService
                        .CreateSuccess($"Added reaction tracker to the [message]({message.JumpLink}).")
                        .Build();

                    await ctx.RespondAsync(embed);
                }
            }
            
            [Command("remove")]
            [Description("Removes a reaction tracker from specified message.")]
            public async Task Remove(CommandContext ctx, [Description("Message to remove reaction tracker from.")] DiscordMessage message)
            {
                if (!InteractiveService.HasReactionHandler<ReactionTrackerHandler>(message.Id))
                {
                    var embed = EmbedService.CreateError("This message doesn't have reaction tracker.").Build();
                    await ctx.RespondAsync(embed);
                }
                else
                {
                    // Remove the reaction handler and its persistent data.
                    await using (var dbContext = DbContextFactory.CreateDbContext())
                    {
                        await InteractiveService.RemoveReactionHandlerAsync(message.Id);
                        var data = await dbContext.ReactionTrackerData.FindAsync(message.Id);
                        dbContext.ReactionTrackerData.Remove(data);
                        await dbContext.SaveChangesAsync();
                    }
                
                    var embed = EmbedService
                        .CreateSuccess($"Removed reaction tracker from the [message]({message.JumpLink}).")
                        .Build();

                    await ctx.RespondAsync(embed);
                }
            }
        }
    }
}