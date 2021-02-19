﻿using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Infrastructure.Interactive;
using UrfRidersBot.Persistence;

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

        [Group("reactionTracker")]
        [Description("Attach to a message to get notified whenever a someone adds or removes a reaction.")]
        public class ReactionTrackerSubmodule : BaseCommandModule
        {
            private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;
            private readonly IInteractiveService _interactiveService;

            public ReactionTrackerSubmodule(
                IDbContextFactory<UrfRidersDbContext> dbContextFactory,
                IInteractiveService interactiveService)
            {
                _dbContextFactory = dbContextFactory;
                _interactiveService = interactiveService;
            }
            
            [Command("add")]
            [Description("Add a reaction tracker to specified message.")]
            public async Task Add(CommandContext ctx, [Description("Message to attach reaction tracker on.")] DiscordMessage message)
            {
                if (_interactiveService.HasReactionHandler(message.Id))
                {
                    var embed = EmbedHelper.CreateError("This message already has some kind of reaction handler.").Build();
                    await ctx.RespondAsync(embed);
                }
                else
                {
                    // Create new reaction handler and create persistent data for it.
                    // TODO: Make this into a service method. I know this is a test command but something like this will be used in the future.
                    await using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        await _interactiveService.AddReactionHandlerAsync<ReactionTrackerHandler>(message.Id);
                        await dbContext.ReactionTrackerData.AddAsync(new ReactionTrackerData(message.Id, ctx.User.Id));
                        await dbContext.SaveChangesAsync();
                    }

                    var embed = EmbedHelper
                        .CreateSuccess($"Added reaction tracker to the [message]({message.JumpLink}).")
                        .Build();

                    await ctx.RespondAsync(embed);
                }
            }
            
            [Command("remove")]
            [Description("Removes a reaction tracker from specified message.")]
            public async Task Remove(CommandContext ctx, [Description("Message to remove reaction tracker from.")] DiscordMessage message)
            {
                if (!_interactiveService.HasReactionHandler<ReactionTrackerHandler>(message.Id))
                {
                    var embed = EmbedHelper.CreateError("This message doesn't have reaction tracker.").Build();
                    await ctx.RespondAsync(embed);
                }
                else
                {
                    // Remove the reaction handler and its persistent data.
                    await using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        await _interactiveService.RemoveReactionHandlerAsync(message.Id);
                        var data = await dbContext.ReactionTrackerData.FindAsync(message.Id);
                        dbContext.ReactionTrackerData.Remove(data);
                        await dbContext.SaveChangesAsync();
                    }
                
                    var embed = EmbedHelper
                        .CreateSuccess($"Removed reaction tracker from the [message]({message.JumpLink}).")
                        .Build();

                    await ctx.RespondAsync(embed);
                }
            }
        }
    }
}