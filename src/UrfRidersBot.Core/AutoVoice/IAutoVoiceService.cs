using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.AutoVoice
{
    public interface IAutoVoiceService
    {
        /// <summary>
        /// Creates new discord voice channel and saves it in the database.
        /// </summary>
        /// <param name="guild">Target guild.</param>
        /// <param name="category">Category to put the new channel in.</param>
        /// <param name="bitrate">Bitrate of the new channel in Kbps.</param>
        /// <returns>Created voice channel.</returns>
        ValueTask<DiscordChannel> CreateAsync(DiscordGuild guild, DiscordChannel? category, int? bitrate);

        /// <summary>
        /// Finds the best name for the <see cref="voiceChannel"/> and if it's different then it modifies the channel to match the name.
        /// </summary>
        /// <param name="voiceChannel">The voice channel to be (potentially) renamed.</param>
        Task UpdateNameAsync(DiscordChannel voiceChannel);

        string GetBestName(DiscordChannel voiceChannel);
    }
}