using System.Threading.Tasks;

namespace UrfRidersBot.Discord.Interactive
{
    public interface IInteractiveService
    {
        Task AddReactionHandlerAsync<T>(ulong messageId) where T : IReactionHandler;
        Task RemoveReactionHandlerAsync(ulong messageId);
        bool HasReactionHandler(ulong messageId);
        bool HasReactionHandler<T>(ulong messageId) where T : IReactionHandler;
    }
}