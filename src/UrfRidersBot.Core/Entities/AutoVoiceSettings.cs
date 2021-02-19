using System.Collections.Generic;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Entities
{
    public class AutoVoiceSettings
    {
        private readonly List<DiscordChannel> _voiceChannels;
        
        public DiscordGuild Guild { get; }
        public bool IsEnabled { get; set; }
        public DiscordChannel? VoiceChannelCreator { get; set; }
        public ICollection<ulong> VoiceChannelIds { get; private set; }
        
        public IReadOnlyCollection<DiscordChannel> VoiceChannels => _voiceChannels.AsReadOnly();

        public AutoVoiceSettings(DiscordGuild guild)
        {
            Guild = guild;

            VoiceChannelIds = new List<ulong>();
            _voiceChannels = new List<DiscordChannel>();
        }

        public void Create()
        {
            _voiceChannels.Add(null);
        }

        public void Delete(DiscordChannel channel)
        {
            _voiceChannels.Remove(channel);
        }

        public void Enable()
        {
            
        }

        public void Disable()
        {
            
        }
    }
}