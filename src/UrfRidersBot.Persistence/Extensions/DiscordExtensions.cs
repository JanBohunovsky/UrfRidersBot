using System;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Persistence.DTOs;

namespace UrfRidersBot.Persistence
{
    // TODO: Replace this with mapper
    internal static class DiscordExtensions
    {
        public static DiscordChannel? FromDTO(this AutoVoiceChannelDTO dto, DiscordGuild guild)
        {
            if (guild.Id != dto.GuildId)
                throw new ArgumentException("Invalid guild.", nameof(guild));
            
            // TODO: Maybe use DiscordClient.GetChannelAsync() ?
            return guild.GetChannel(dto.VoiceChannelId);
        }

        public static DiscordChannel? FromDTO(this AutoVoiceChannelDTO dto, DiscordClient client)
        {
            return client.Guilds[dto.GuildId].GetChannel(dto.VoiceChannelId);
        }

        public static AutoVoiceChannelDTO ToDTO(this DiscordChannel channel)
        {
            return new AutoVoiceChannelDTO(channel.GuildId, channel.Id);
        }
    }
}