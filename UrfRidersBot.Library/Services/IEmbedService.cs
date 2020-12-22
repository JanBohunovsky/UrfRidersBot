using Discord;

namespace UrfRidersBot.Library
{
    public interface IEmbedService
    {
        /// <summary>
        /// Creates a basic embed with <see cref="BotConfiguration.EmbedColor"/> color.
        /// </summary>
        EmbedBuilder Basic(string? description = null, string? title = null);
        
        /// <summary>
        /// Creates a success embed with green color and appends <see cref="EmoteConfiguration.Ok"/> to the title..
        /// </summary>
        EmbedBuilder Success(string? description = null, string title = "Success");
        
        /// <summary>
        /// Creates an error embed with yellow color and appends <see cref="EmoteConfiguration.Error"/> to the title..
        /// </summary>
        EmbedBuilder Error(string? description = null, string? title = "Error");
        
        /// <summary>
        /// Creates a critical error embed with red color and appends <see cref="EmoteConfiguration.Critical"/> to the title..
        /// </summary>
        EmbedBuilder CriticalError(string? description = null, string? title = "Critical Error");
    }
}