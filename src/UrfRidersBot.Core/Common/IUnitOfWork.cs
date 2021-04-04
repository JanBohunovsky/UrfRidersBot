using System;
using System.Threading.Tasks;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Core.ColorRole;
using UrfRidersBot.Core.ReactionRoles;
using UrfRidersBot.Core.Settings;

namespace UrfRidersBot.Core.Common
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IAutoVoiceSettingsRepository AutoVoiceSettings { get; }
        IGuildSettingsRepository GuildSettings { get; }
        IReactionRoleRepository ReactionRoles { get; }
        IColorRoleRepository ColorRoles { get; }
        Task CompleteAsync();
    }
}