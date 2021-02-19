using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Infrastructure.Interactive;
using UrfRidersBot.Persistence;

namespace UrfRidersBot.Infrastructure
{
    internal partial class InteractiveService : IInteractiveService
    {
        private readonly DiscordClient _client;
        private readonly IServiceProvider _provider;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;
        private readonly ILogger<InteractiveService> _logger;
        
        private readonly Dictionary<ulong, IReactionHandler> _reactionHandlers;

        public InteractiveService(
            DiscordClient client,
            IServiceProvider provider,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory,
            ILogger<InteractiveService> logger)
        {
            _provider = provider;
            _dbContextFactory = dbContextFactory;
            _client = client;
            _logger = logger;

            _reactionHandlers = new Dictionary<ulong, IReactionHandler>();
        }

        public async Task AddReactionHandlerAsync<T>(ulong messageId) where T : IReactionHandler
        {
            var handlerInstance = ActivatorUtilities.CreateInstance<T>(_provider);
            var handlerInfo = new ReactionHandlerInfo(messageId, handlerInstance.GetType().FullName!);
            
            await using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.ActiveReactionHandlers.AddAsync(handlerInfo);
            await dbContext.SaveChangesAsync();
            
            _reactionHandlers[messageId] = handlerInstance;
        }

        public async Task RemoveReactionHandlerAsync(ulong messageId)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            var handlerInfo = await dbContext.ActiveReactionHandlers.FindAsync(messageId);
            if (handlerInfo != null)
            {
                dbContext.ActiveReactionHandlers.Remove(handlerInfo);
                await dbContext.SaveChangesAsync();
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