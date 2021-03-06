﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.Commands.Modules
{
    [RequireGuildRank(GuildRank.Admin)]
    [Group("autoVoice")]
    [Description("Automatically create new voice channel whenever all are taken.")]
    public class AutoVoiceModule : BaseCommandModule
    {
        private readonly IAutoVoiceService _autoVoiceService;

        public AutoVoiceModule(IAutoVoiceService autoVoiceService)
        {
            _autoVoiceService = autoVoiceService;
        }

        [GroupCommand]
        [Description("Current Auto Voice status and more useful information.")]
        public async Task Information(CommandContext ctx)
        {
            var channels = (await _autoVoiceService.GetByGuildAsync(ctx.Guild)).ToList();

            if (!channels.Any())
                throw new InvalidOperationException("Auto Voice is disabled on this server.");

            var channelBuilder = new StringBuilder();
            foreach (var channel in channels)
            {
                channelBuilder.AppendLine($"Name: `{channel.Name}`");
                channelBuilder.AppendLine($"ID: `{channel.Id}`");
                channelBuilder.AppendLine($"Users: {channel.Users.Count()}");
                
                foreach (var user in channel.Users)
                {
                    if (user == null)
                    {
                        channelBuilder.AppendLine("*Unknown user*");
                        continue;
                    }
                    
                    channelBuilder.Append(user.Mention);

                    if (user.Presence.Activities.Any())
                    {
                        channelBuilder.Append(": ");
                        channelBuilder.AppendJoin(", ", user.Presence.Activities.Select(x => $"{GetActivityText(x.ActivityType)} **{x.Name}**"));
                    }

                    channelBuilder.AppendLine();
                }

                channelBuilder.AppendLine();
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Current auto voice channels on {ctx.Guild.Name}",
                Color = UrfRidersColor.Blue,
                Description = channelBuilder.ToString(),
            };
            await ctx.RespondAsync(embed.Build());
        }

        private string GetActivityText(ActivityType activityType)
        {
            return activityType switch
            {
                ActivityType.Playing     => "Playing",
                ActivityType.Watching    => "Watching",
                ActivityType.Streaming   => "Streaming",
                ActivityType.Competing   => "Competing in",
                ActivityType.ListeningTo => "Listening to",
                ActivityType.Custom      => "Custom:",
                _                        => "Unknown:",
            };
        }
        
        [Command("enable")]
        [Description("Enables the module and creates a new voice channel.")]
        public async Task Enable(CommandContext ctx, [Description("Where the voice channel should be created.")] DiscordChannel? category = null)
        {
            if (category != null && !category.IsCategory)
            {
                throw new ArgumentException("Channel must be a category.", nameof(category));
            }

            var voiceChannel = await _autoVoiceService.EnableForGuildAsync(ctx.Guild, category);

            var sb = new StringBuilder();
            sb.AppendLine("Auto Voice has been enabled.");
            sb.Append($"Created a new voice channel `{voiceChannel.Name}`");

            if (voiceChannel.Parent != null)
            {
                sb.Append($" under category `{voiceChannel.Parent.Name}`");
            }

            sb.AppendLine(".");
            sb.Append("Whenever someone joins this channel, a new one will be created and whenever one of these channels is empty, it will be deleted.");

            await ctx.RespondAsync(EmbedHelper.CreateSuccess(sb.ToString()).Build());
        }
        
        [Command("disable")]
        [Description("Disables the module and deletes all the voice channels created by this module.")]
        public async Task Disable(CommandContext ctx)
        {
            var count = await _autoVoiceService.DisableForGuildAsync(ctx.Guild);

            var embed = EmbedHelper
                .CreateSuccess("Auto Voice has been disabled.")
                .WithFooter($"Deleted {count} channel(s)");

            await ctx.RespondAsync(embed.Build());
        }
    }
}