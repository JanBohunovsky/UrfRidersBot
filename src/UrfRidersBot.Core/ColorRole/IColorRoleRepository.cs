using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.ColorRole
{
    public interface IColorRoleRepository : IRepository
    {
        ValueTask<DiscordRole?> GetByUserAsync(DiscordUser user);
        ValueTask<bool> AddAsync(DiscordRole role, DiscordUser user);
        Task RemoveAsync(DiscordRole role);
    }
}