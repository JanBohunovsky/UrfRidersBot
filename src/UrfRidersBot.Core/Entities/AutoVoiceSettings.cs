using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Entities
{
    public class AutoVoiceSettings
    {
        private List<AutoVoiceChannel> _voiceChannels;
        
        public ulong GuildId { get; private set; }
        public int? Bitrate { get; set; }
        public ulong? ChannelCreatorId { get; set; }
        public IReadOnlyCollection<AutoVoiceChannel> VoiceChannels => _voiceChannels.AsReadOnly();

        public AutoVoiceSettings(ulong guildId)
        {
            GuildId = guildId;

            _voiceChannels = new List<AutoVoiceChannel>();
        }

        /// <summary>
        /// Adds current channel creator to <see cref="VoiceChannels"/> and sets <see cref="voiceChannel"/> as the new channel creator.
        /// </summary>
        /// <param name="voiceChannel"></param>
        public void AddChannel(DiscordChannel voiceChannel)
        {
            var channelCreator = GetChannelCreator(voiceChannel.Guild);
            if (channelCreator is null)
            {
                return;
            }
            
            _voiceChannels.Add(AutoVoiceChannel.FromDiscord(channelCreator));
            ChannelCreatorId = voiceChannel.Id;
        }

        public bool RemoveChannel(ulong voiceChannelId)
        {
            return _voiceChannels.RemoveAll(v => v.GuildId == GuildId && v.VoiceChannelId == voiceChannelId) > 0;
        }

        public bool RemoveChannel(DiscordChannel voiceChannel) => RemoveChannel(voiceChannel.Id);

        public int RemoveAllChannels()
        {
            var count = _voiceChannels.Count;
            _voiceChannels.Clear();
            
            return count;
        }
        
        public bool ContainsChannel(DiscordChannel voiceChannel)
        {
            return _voiceChannels.Any(v => v.GuildId == GuildId && v.VoiceChannelId == voiceChannel.Id);
        }

        public DiscordChannel? GetChannelCreator(DiscordGuild guild)
        {
            return ChannelCreatorId is null ? null : guild.GetChannel(ChannelCreatorId.Value);
        }

        public IReadOnlyCollection<DiscordChannel> GetVoiceChannels(DiscordGuild guild)
        {
            return VoiceChannels
                .Select(v => v.ToDiscord(guild))
                .ToList()
                .AsReadOnly();
        }
    }
}