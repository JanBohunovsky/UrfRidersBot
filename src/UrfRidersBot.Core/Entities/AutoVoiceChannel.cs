﻿using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Entities
{
    public class AutoVoiceChannel
    {
        public ulong VoiceChannelId { get; private set; }
        public ulong GuildId { get; private set; }
        
        public AutoVoiceChannel(ulong voiceChannelId, ulong guildId)
        {
            VoiceChannelId = voiceChannelId;
            GuildId = guildId;
        }

        public DiscordChannel ToDiscord(DiscordGuild guild)
        {
            return guild.GetChannel(VoiceChannelId);
        }

        public DiscordChannel ToDiscord(DiscordClient client)
        {
            return client.Guilds[GuildId].GetChannel(VoiceChannelId);
        }

        public static AutoVoiceChannel FromDiscord(DiscordChannel channel)
        {
            return new AutoVoiceChannel(channel.Id, channel.GuildId);
        }
    }
}