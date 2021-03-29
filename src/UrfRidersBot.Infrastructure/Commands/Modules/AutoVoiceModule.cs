using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Infrastructure.Commands.Attributes;

namespace UrfRidersBot.Infrastructure.Commands.Modules
{
    [RequireGuildRank(GuildRank.Admin)]
    [Group("autoVoice")]
    [Description("Automatically create new voice channel whenever all are taken.")]
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class AutoVoiceModule : BaseCommandModule
    {
        private readonly IAutoVoiceService _autoVoiceService;
        private readonly IUnitOfWork _unitOfWork;

        public AutoVoiceModule(IAutoVoiceService autoVoiceService, IUnitOfWork unitOfWork)
        {
            _autoVoiceService = autoVoiceService;
            _unitOfWork = unitOfWork;
        }

        [GroupCommand]
        [Description("Current Auto Voice status and more useful information.")]
        public async Task Information(CommandContext ctx)
        {
            var settings = await _unitOfWork.AutoVoiceSettings.GetAsync(ctx.Guild);
            
            if (settings?.ChannelCreatorId is null)
            {
                throw new InvalidOperationException("Auto Voice is disabled on this server.");
            }
            
            var channels = settings.GetVoiceChannels(ctx.Guild);
            DiscordChannel channelCreator = settings.GetChannelCreator(ctx.Guild)!;
            
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
                Title = $"Auto Voice on {ctx.Guild.Name}",
                Color = UrfRidersColor.Cyan
            };
            
            embed.AddField("Channel Creator", $"Name: `{channelCreator.Name}`\nID: `{channelCreator.Id}`", true);
            embed.AddField("Bitrate", $"{settings.Bitrate ?? 64} Kbps", true);
            
            if (channels.Any())
            {
                embed.AddField("Channels", channelBuilder.ToString());
            }
            
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

            var settings = await _unitOfWork.AutoVoiceSettings.GetOrCreateAsync(ctx.Guild);
            if (settings.ChannelCreatorId is not null)
            {
                throw new InvalidOperationException("Auto Voice is already enabled on this server.");
            }

            var voiceChannelCreator = await _autoVoiceService.CreateAsync(ctx.Guild, category, settings.Bitrate);

            settings.ChannelCreatorId = voiceChannelCreator.Id;
            await _unitOfWork.CompleteAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Auto Voice has been enabled.");
            sb.Append($"Created a new voice channel `{voiceChannelCreator.Name}`");

            if (voiceChannelCreator.Parent != null)
            {
                sb.Append($" under category `{voiceChannelCreator.Parent.Name}`");
            }

            sb.AppendLine(".");
            sb.Append("Whenever someone joins this channel, a new one will be created and whenever one of these channels is empty, it will be deleted.");

            await ctx.RespondAsync(EmbedHelper.CreateSuccess(sb.ToString()).Build());
        }
        
        [Command("disable")]
        [Description("Disables the module and deletes all the voice channels created by this module.")]
        public async Task Disable(CommandContext ctx)
        {
            var settings = await _unitOfWork.AutoVoiceSettings.GetAsync(ctx.Guild);
            if (settings?.ChannelCreatorId is null)
            {
                throw new InvalidOperationException("Auto Voice is already disabled on this server.");
            }

            foreach (var voiceChannel in settings.GetVoiceChannels(ctx.Guild))
            {
                await voiceChannel.DeleteAsync();
            }
            var count = settings.RemoveAllChannels();

            var channelCreator = settings.GetChannelCreator(ctx.Guild);
            if (channelCreator is not null)
            {
                await channelCreator.DeleteAsync();
                count++;
            }

            settings.ChannelCreatorId = null;
            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper
                .CreateSuccess("Auto Voice has been disabled.")
                .WithFooter($"Deleted {count} channel(s)");

            await ctx.RespondAsync(embed.Build());
        }
    }
}