using System;
using System.Threading.Tasks;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IAutoVoiceChannelRepository AutoVoiceChannels { get; }
        IGuildSettingsRepository GuildSettings { get; }
        IReactionRoleRepository ReactionRoles { get; }
        IColorRoleRepository ColorRoles { get; }
        Task CompleteAsync();
    }
}