using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IReactionTrackerDataRepository
    {
        Task AddAsync(ReactionTrackerData data);
        ValueTask<ReactionTrackerData?> GetByMessageAsync(DiscordMessage message);
        void Remove(ReactionTrackerData data);
    }
}