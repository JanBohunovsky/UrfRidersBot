using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IAutoVoiceService
    {
        ValueTask<DiscordChannel> EnableForGuildAsync(DiscordGuild guild, DiscordChannel? category = null);
        ValueTask<int> DisableForGuildAsync(DiscordGuild guild);
        ValueTask<IEnumerable<DiscordChannel>> GetVoiceChannelsAsync(DiscordGuild guild);

        /// <summary>
        /// Creates new discord voice channel and saves it in the database.
        /// </summary>
        /// <param name="template">Template channel is an existing auto voice channel.</param>
        /// <returns>Created voice channel.</returns>
        ValueTask<DiscordChannel> CreateVoiceChannelAsync(DiscordChannel template);

        /// <summary>
        /// Deletes specified <see cref="voiceChannel"/> and removes it from the database.
        /// </summary>
        /// <param name="voiceChannel">The voice channel to be deleted.</param>
        Task DeleteVoiceChannelAsync(DiscordChannel voiceChannel);

        /// <summary>
        /// Finds the best name for the <see cref="voiceChannel"/> and if it's different then it modifies the channel to match the name.
        /// </summary>
        /// <param name="voiceChannel">The voice channel to be (potentially) renamed.</param>
        Task UpdateVoiceChannelNameAsync(DiscordChannel voiceChannel);
        
        /// <summary>
        /// Finds an auto voice channel the <see cref="user"/> is connected to, if any.
        /// </summary>
        /// <param name="client">Discord client.</param>
        /// <param name="user">Target user.</param>
        /// <returns>Auto voice channel the <see cref="user"/> is connected to.</returns>
        ValueTask<DiscordChannel?> FindVoiceChannelAsync(DiscordUser user);
        
        /// <summary>
        /// Checks if the <see cref="voiceChannel"/> is an auto voice channel.
        /// </summary>
        /// <param name="voiceChannel">Voice channel to check.</param>
        /// <returns>True if it is an auto voice channel.</returns>
        ValueTask<bool> ContainsVoiceChannelAsync(DiscordChannel? voiceChannel);
        
        /// <summary>
        /// Checks if the <see cref="voiceChannel"/> is the voice channel that creates new channels.
        /// </summary>
        /// <param name="voiceChannel">Voice channel to check.</param>
        /// <returns>True if it is a voice channel that creates new channels.</returns>
        ValueTask<bool> IsVoiceChannelCreatorAsync(DiscordChannel voiceChannel);

        /// <summary>
        /// Make the data stored in database up to date.
        /// </summary>
        /// <returns></returns>
        Task CatchUpAsync();
    }
}