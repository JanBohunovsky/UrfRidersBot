using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrfRidersBot.Discord.Interactive
{
    internal partial class InteractiveService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageReactionAdded += OnMessageReactionAdded;
            _client.MessageReactionRemoved += OnMessageReactionRemoved;
            
            // Create reaction handler instances for each active handler/message
            await using var dbContext = _dbContextFactory.CreateDbContext();

            await foreach (var info in dbContext.ActiveReactionHandlers)
            {
                var type = Type.GetType(info.TypeName);
                if (type == null)
                {
                    _logger.LogWarning("Could not find type '{Type}'", info.TypeName);
                    continue;
                }

                var instance = ActivatorUtilities.CreateInstance(_provider, type);
                _reactionHandlers[info.MessageId] = (IReactionHandler)instance;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.MessageReactionAdded -= OnMessageReactionAdded;
            _client.MessageReactionRemoved -= OnMessageReactionRemoved;
            
            return Task.CompletedTask;
        }

        private async Task OnMessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            if (e.User.IsCurrent)
                return;
            
            // Find reaction handler
            var handler = _reactionHandlers.GetValueOrDefault(e.Message.Id);
            if (handler == null)
                return;

            await handler.ReactionAdded(e.Message, e.User, e.Emoji);
        }

        private async Task OnMessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            if (e.User.IsCurrent)
                return;
            
            // Find reaction handler
            var handler = _reactionHandlers.GetValueOrDefault(e.Message.Id);
            if (handler == null)
                return;

            await handler.ReactionRemoved(e.Message, e.User, e.Emoji);
        }
    }
}