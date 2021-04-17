using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.AutoVoice
{
    public class AutoVoiceSettings
    {
        private readonly List<DiscordChannel> _voiceChannels;
        
        /// <summary>
        /// Voice channel bitrate in Kbps
        /// </summary>
        public int? Bitrate { get; set; }
        
        public DiscordChannel? ChannelCreator { get; set; }
        
        public IReadOnlyCollection<DiscordChannel> VoiceChannels => _voiceChannels.AsReadOnly();

        public AutoVoiceSettings(IEnumerable<DiscordChannel> voiceChannels)
        {
            _voiceChannels = voiceChannels.ToList();
        }

        /// <summary>
        /// Adds current channel creator to <see cref="VoiceChannels"/> and sets <see cref="voiceChannel"/> as the new channel creator.
        /// </summary>
        /// <param name="voiceChannel"></param>
        public void AddChannel(DiscordChannel voiceChannel)
        {
            if (ChannelCreator is null)
            {
                return;
            }
            
            _voiceChannels.Add(ChannelCreator);
            ChannelCreator = voiceChannel;
        }

        public bool RemoveChannel(DiscordChannel voiceChannel) => _voiceChannels.Remove(voiceChannel);
        
        public bool ContainsChannel(DiscordChannel voiceChannel) => _voiceChannels.Contains(voiceChannel);

        public int RemoveAllChannels()
        {
            var count = _voiceChannels.Count;
            _voiceChannels.Clear();
            
            return count;
        }
        
    }
}