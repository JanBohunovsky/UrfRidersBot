using System.Collections.Generic;
using System.Threading.Tasks;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IReactionHandlerInfoRepository
    {
        ValueTask<IEnumerable<ReactionHandlerInfo>> GetAllAsync();
        ValueTask<ReactionHandlerInfo?> GetByMessageId(ulong messageId);
        Task AddAsync(ReactionHandlerInfo handlerInfo);
        void Remove(ReactionHandlerInfo handlerInfo);
    }
}