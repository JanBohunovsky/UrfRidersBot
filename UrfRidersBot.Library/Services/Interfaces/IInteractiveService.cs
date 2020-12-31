using System.Collections.Generic;

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
        void AddReactionHandler<T>(ulong messageId) where T : IReactionHandler;

        void RemoveReactionHandler(ulong messageId);
        
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