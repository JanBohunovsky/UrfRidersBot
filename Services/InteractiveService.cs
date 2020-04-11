using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UrfRiders.Interactive;

namespace UrfRiders.Services
{
    public class InteractiveService
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger _logger;

        private readonly ILiteCollection<IReactionHandler> _data;

        public InteractiveService(DiscordSocketClient client, LiteDatabase database, ILogger<InteractiveService> logger)
        {
            _client = client;
            _logger = logger;

            _data = database.GetCollection<IReactionHandler>();

            _client.ReactionAdded += ReactionAdded;
            _client.ReactionRemoved += ReactionRemoved;
        }

        #region Reactions
        public void SetHandler(ulong messageId, IReactionHandler handler) => _data.Upsert(messageId, handler);
        public IReactionHandler GetHandler(ulong messageId) => _data.FindById(messageId);
        public void RemoveHandler(ulong messageId) => _data.Delete(messageId);

        public T GetHandler<T>(ulong messageId) where T : IReactionHandler
        {
            var handler = GetHandler(messageId);
            if (handler == null)
                return default;
            if (!(handler is T outHandler))
                return default;

            return outHandler;
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id)
                return;

            var handler = GetHandler(cachedMessage.Id);
            if (handler == null)
                return;

            var message = await cachedMessage.GetOrDownloadAsync();
            var user = reaction.User.GetValueOrDefault(null) ?? _client.GetUser(reaction.UserId);

            if (handler.RunMode == RunMode.Async)
                _ = Task.Run(async () => await handler.ReactionAdded(message, user, reaction.Emote));
            else
                await handler.ReactionAdded(message, user, reaction.Emote);
        }

        private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id)
                return;

            var handler = GetHandler(cachedMessage.Id);
            if (handler == null)
                return;

            var message = await cachedMessage.GetOrDownloadAsync();
            var user = reaction.User.GetValueOrDefault(null) ?? _client.GetUser(reaction.UserId);

            if (handler.RunMode == RunMode.Async)
                _ = Task.Run(async () => await handler.ReactionRemoved(message, user, reaction.Emote));
            else
                await handler.ReactionRemoved(message, user, reaction.Emote);
        }
        #endregion
    }
}