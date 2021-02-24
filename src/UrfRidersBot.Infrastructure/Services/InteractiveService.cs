using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Infrastructure.Interactive;

namespace UrfRidersBot.Infrastructure
{
    internal partial class InteractiveService : IInteractiveService
    {
        private readonly DiscordClient _client;
        private readonly IServiceProvider _provider;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger<InteractiveService> _logger;
        
        private readonly Dictionary<ulong, IReactionHandler> _reactionHandlers;

        public InteractiveService(
            DiscordClient client,
            IServiceProvider provider,
            IUnitOfWorkFactory unitOfWorkFactory,
            ILogger<InteractiveService> logger)
        {
            _client = client;
            _provider = provider;
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;

            _reactionHandlers = new Dictionary<ulong, IReactionHandler>();
        }

        public async Task AddReactionHandlerAsync<T>(ulong messageId) where T : IReactionHandler
        {
            var handlerInstance = ActivatorUtilities.CreateInstance<T>(_provider);
            var handlerInfo = new ReactionHandlerInfo(messageId, handlerInstance.GetType().FullName!);
            
            await using var unitOfWork = _unitOfWorkFactory.Create();
            await unitOfWork.ActiveReactionHandlers.AddAsync(handlerInfo);
            await unitOfWork.CompleteAsync();
            
            _reactionHandlers[messageId] = handlerInstance;
        }

        public async Task RemoveReactionHandlerAsync(ulong messageId)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var handlerInfo = await unitOfWork.ActiveReactionHandlers.GetByMessageId(messageId);
            if (handlerInfo != null)
            {
                unitOfWork.ActiveReactionHandlers.Remove(handlerInfo);
                await unitOfWork.CompleteAsync();
            }
            
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