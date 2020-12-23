﻿using Discord;

namespace UrfRidersBot.Library
{
    public interface IEmbedService
    {
        /// <summary>
        /// Creates a basic embed with <see cref="BotConfiguration.EmbedColor"/> color.
        /// </summary>
        EmbedBuilder Basic(string? description = null, string? title = null);
        
        /// <summary>
        /// Creates a success embed with green color and sets the <see cref="title"/> as embed's author with a green checkmark image as icon.
        /// </summary>
        EmbedBuilder Success(string? description = null, string title = "Success");
        
        /// <summary>
        /// Creates an error embed with yellow color and sets the <see cref="title"/> as embed's author with a yellow exclamation triangle image as icon.
        /// </summary>
        EmbedBuilder Error(string? description = null, string? title = "Error");
        
        /// <summary>
        /// Creates a critical error embed with red color and sets the <see cref="title"/> as embed's author with a red exclamation quad image as icon.
        /// </summary>
        EmbedBuilder CriticalError(string? description = null, string? title = "Critical Error");
    }
}