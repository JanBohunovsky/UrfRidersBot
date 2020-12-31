﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace UrfRidersBot.Library
{
    public interface IInteractiveService
    {
        internal Dictionary<ulong, IReactionHandler> ReactionHandlers { get; }

        /// <summary>
        /// Creates new <see cref="IReactionHandler"/> of type <see cref="T"/> and sets it to message with ID of <see cref="messageId"/>.
        /// <para>
        /// Warning: This will replace any existing handlers!
        /// </para>
        /// </summary>
        Task AddReactionHandlerAsync<T>(ulong messageId) where T : IReactionHandler;

        Task RemoveReactionHandlerAsync(ulong messageId);
        
        /// <summary>
        /// Checks if the message has a reaction handler of any type.
        /// </summary>
        bool HasReactionHandler(ulong messageId);
        
        /// <summary>
        /// Checks if the message has a reaction handler of type <see cref="T"/>.
        /// </summary>
        bool HasReactionHandler<T>(ulong messageId) where T : IReactionHandler;
    }
}