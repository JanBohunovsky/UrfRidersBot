using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Commands.AutoVoice
{
    public class Information : ICommand<AutoVoiceGroup>
    {
        private readonly IUnitOfWork _unitOfWork;

        public bool Ephemeral => false;
        public string Name => "info";
        public string Description => "Lists current auto voice channels on this server and the users in them.";

        public Information(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var settings = await _unitOfWork.AutoVoiceSettings.GetAsync(context.Guild);
            
            if (settings?.ChannelCreatorId is null)
            {
                return CommandResult.InvalidOperation("Auto Voice is disabled on this server.");
            }
            
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Auto Voice on {context.Guild.Name}",
                Color = UrfRidersColor.Cyan
            };
            
            var channelCreator = settings.GetChannelCreator(context.Guild)!;
            embed.AddField("Channel Creator", $"Name: `{channelCreator.Name}`\nID: `{channelCreator.Id}`", true);
            
            embed.AddField("Bitrate", $"{settings.Bitrate ?? 64} Kbps", true);
            
            var channels = settings.GetVoiceChannels(context.Guild);
            if (channels.Any())
            {
                embed.AddField("Channels", CreateChannelList(channels));
            }
            
            await context.RespondAsync(embed);
            
            return CommandResult.NoAction;
        }

        private string CreateChannelList(IEnumerable<DiscordChannel> channels)
        {
            var sb = new StringBuilder();
            foreach (var channel in channels)
            {
                AddChannel(sb, channel);
            }

            return sb.ToString();
        }

        private void AddChannel(StringBuilder sb, DiscordChannel channel)
        {
            sb.AppendLine($"Name: `{channel.Name}`");
            sb.AppendLine($"ID: `{channel.Id}`");
            sb.AppendLine($"Users: {channel.Users.Count()}");
                
            foreach (var member in channel.Users)
            {
                AddMember(sb, member);
            }

            sb.AppendLine();
        }

        private void AddMember(StringBuilder sb, DiscordMember member)
        {      
            sb.Append(member.Mention);

            if (member.Presence.Activities.Any())
            {
                sb.Append(": ");
                sb.AppendJoin(", ", member.Presence.Activities.Select(x => $"{GetActivityText(x.ActivityType)} **{x.Name}**"));
            }

            sb.AppendLine();
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
    }
}