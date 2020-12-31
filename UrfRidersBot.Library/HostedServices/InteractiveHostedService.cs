using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrfRidersBot.Library
{
    internal class InteractiveHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IInteractiveService _interactiveService;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;
        private readonly IServiceProvider _provider;
        private readonly ILogger<InteractiveHostedService> _logger;

        public InteractiveHostedService(
            DiscordSocketClient discord,
            IInteractiveService interactiveService,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory, 
            IServiceProvider provider,
            ILogger<InteractiveHostedService> logger)
        {
            _discord = discord;
            _interactiveService = interactiveService;
            _dbContextFactory = dbContextFactory;
            _provider = provider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _discord.ReactionAdded += ReactionAdded;
            _discord.ReactionRemoved += ReactionRemoved;
            
            // Create reaction handler instances for each active handler.
            await using var dbContext = _dbContextFactory.CreateDbContext();
            
            await foreach (var info in dbContext.ActiveReactionHandlers)
            {
                var type = Type.GetType(info.TypeName);
                if (type == null)
                {
                    _logger.LogWarning("Could not find type '{typeName}'.", info.TypeName);
                    continue;
                }
                
                var instance = ActivatorUtilities.CreateInstance(_provider, type);
                _interactiveService.ReactionHandlers[info.MessageId] = (IReactionHandler)instance;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _discord.ReactionAdded -= ReactionAdded;
            _discord.ReactionRemoved -= ReactionRemoved;
            
            // TODO: Move this shit to InteractiveService.cs
            
            // Save all active handlers to the database so that we can create them on the next start.
            await using var dbContext = _dbContextFactory.CreateDbContext();

            // Store a collection of new reaction handlers (compared with database).
            var newHandlers = _interactiveService.ReactionHandlers.Keys.ToHashSet();
            
            // Delete any saved handlers from db if they are no longer active.
            await foreach (var info in dbContext.ActiveReactionHandlers)
            {
                if (_interactiveService.ReactionHandlers.ContainsKey(info.MessageId))
                {
                    // This message already has stored reaction handler in the database, so we'll remove it from the "new handlers" list.
                    newHandlers.Remove(info.MessageId);
                }
                else
                {
                    // This message no longer has a reaction handler so we need to remove it from the database.
                    dbContext.ActiveReactionHandlers.Remove(info);
                }
            }
            
            // Insert new handlers.
            foreach (var messageId in newHandlers)
            {
                var typeName = _interactiveService.ReactionHandlers[messageId].GetType().FullName;
                var info = new ReactionHandlerInfo(messageId, typeName!);
                await dbContext.ActiveReactionHandlers.AddAsync(info);
            }

            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _discord.CurrentUser.Id)
                return;

            // Find reaction handler
            var handler = _interactiveService.ReactionHandlers.GetValueOrDefault(cachedMessage.Id);
            if (handler == null)
                return;

            // Get message and user
            var message = await cachedMessage.GetOrDownloadAsync();
            var user = _discord.GetUser(reaction.UserId);

            // Run in different thread
            _ = Task.Run(async () => await handler.ReactionAdded(message, user, reaction.Emote));
        }

        private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _discord.CurrentUser.Id)
                return;

            // Find reaction handler
            var handler = _interactiveService.ReactionHandlers.GetValueOrDefault(cachedMessage.Id);
            if (handler == null)
                return;

            // Get message and user
            var message = await cachedMessage.GetOrDownloadAsync();
            var user = _discord.GetUser(reaction.UserId);

            // Run in different thread
            _ = Task.Run(async () => await handler.ReactionRemoved(message, user, reaction.Emote));
        }
    }
}