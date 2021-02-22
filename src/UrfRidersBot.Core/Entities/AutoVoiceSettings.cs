using System.Collections.Generic;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Entities
{
    public class AutoVoiceSettings
    {
        private readonly List<AutoVoiceChannel> _voiceChannels;
        
        public ulong GuildId { get; private set; }
        public bool IsEnabled { get; private set; }
        public ulong? VoiceChannelCreatorId { get; private set; }
        
        public IReadOnlyCollection<AutoVoiceChannel> VoiceChannels => _voiceChannels.AsReadOnly();

        public AutoVoiceSettings(ulong guildId)
        {
            GuildId = guildId;
            
            _voiceChannels = new List<AutoVoiceChannel>();
        }

        public DiscordChannel GetVoiceChannelCreator(DiscordGuild guild)
        {
            return guild.Channels[VoiceChannelCreatorId!.Value];
        }

        public void Enable(DiscordChannel voiceChannelCreator)
        {
            IsEnabled = true;
            VoiceChannelCreatorId = voiceChannelCreator.Id;
        }

        public void Disable()
        {
            IsEnabled = false;
            VoiceChannelCreatorId = null;
            _voiceChannels.Clear();
        }

        public void AddChannel(DiscordChannel newVoiceChannelCreator)
        {
            // Convert voice channel creator into auto voice channel and set the new creator channel.
            _voiceChannels.Add(new AutoVoiceChannel(VoiceChannelCreatorId!.Value, GuildId));
            VoiceChannelCreatorId = newVoiceChannelCreator.Id;
        }

        public void RemoveChannel(ulong voiceChannelId)
        {
            _voiceChannels.RemoveAll(x => x.VoiceChannelId == voiceChannelId);
        }
    }
}