using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Interactive;

namespace UrfRidersBot
{
    public partial class InteractiveService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.ReactionAdded += OnReactionAdded;
            _client.ReactionRemoved += OnReactionRemoved;
            
            // Create reaction handler instances for each active handler/message
            await using var dbContext = _dbContextFactory.CreateDbContext();

            await foreach (var info in dbContext.ActiveReactionHandlers)
            {
                var type = Type.GetType(info.TypeName);
                if (type == null)
                {
                    _logger.LogWarning("Could not find type '{type}'.", info.TypeName);
                    continue;
                }

                var instance = ActivatorUtilities.CreateInstance(_provider, type);
                _reactionHandlers[info.MessageId] = (IReactionHandler)instance;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.ReactionAdded -= OnReactionAdded;
            _client.ReactionRemoved -= OnReactionRemoved;
            
            return Task.CompletedTask;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> rawMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id)
                return;
            
            // Find reaction handler
            var handler = _reactionHandlers.GetValueOrDefault(rawMessage.Id);
            if (handler == null)
                return;
            
            // Get message and user
            var message = await rawMessage.GetOrDownloadAsync();
            var user = _client.GetUser(reaction.UserId);
            
            // Run in different thread
            _ = Task.Run(async () => await handler.ReactionAdded(message, user, reaction.Emote));
        }

        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> rawMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id)
                return;
            
            // Find reaction handler
            var handler = _reactionHandlers.GetValueOrDefault(rawMessage.Id);
            if (handler == null)
                return;
            
            // Get message and user
            var message = await rawMessage.GetOrDownloadAsync();
            var user = _client.GetUser(reaction.UserId);
            
            // Run in different thread
            _ = Task.Run(async () => await handler.ReactionRemoved(message, user, reaction.Emote));
        }
    }
}