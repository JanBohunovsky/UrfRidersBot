using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace UrfRidersBot.Library
{
    internal class InteractiveService : IInteractiveService
    {
        private readonly IServiceProvider _provider;
        private readonly Dictionary<ulong, IReactionHandler> _reactionHandlers;

        public InteractiveService(IServiceProvider provider)
        {
            _provider = provider;
            
            _reactionHandlers = new Dictionary<ulong, IReactionHandler>();
        }
        
        Dictionary<ulong, IReactionHandler> IInteractiveService.ReactionHandlers => _reactionHandlers;

        public void AddReactionHandler<T>(ulong messageId) where T : IReactionHandler
        {
            _reactionHandlers[messageId] = ActivatorUtilities.CreateInstance<T>(_provider);
        }

        public void RemoveReactionHandler(ulong messageId)
        {
            _reactionHandlers.Remove(messageId);
        }

        public bool HasReactionHandler(ulong messageId)
        {
            return _reactionHandlers.ContainsKey(messageId);
        }

        public bool HasReactionHandler<T>(ulong messageId) where T : IReactionHandler
        {
            return _reactionHandlers.GetValueOrDefault(messageId) is T;
        }
    }
}