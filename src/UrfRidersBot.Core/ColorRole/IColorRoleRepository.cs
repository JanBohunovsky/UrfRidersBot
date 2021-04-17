using DSharpPlus.Entities;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.ColorRole
{
    public interface IColorRoleRepository : IRepository
    {
        DiscordRole? GetByUser(DiscordUser user);
        bool Add(DiscordRole role, DiscordUser user);
        void Remove(DiscordRole role);
    }
}