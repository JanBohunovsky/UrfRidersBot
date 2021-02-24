using System;
using System.Threading.Tasks;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IAutoVoiceChannelRepository AutoVoiceChannels { get; }
        IGuildSettingsRepository GuildSettings { get; }
        IReactionTrackerDataRepository ReactionTrackerData { get; }
        IReactionHandlerInfoRepository ActiveReactionHandlers { get; }
        Task CompleteAsync();
    }
}