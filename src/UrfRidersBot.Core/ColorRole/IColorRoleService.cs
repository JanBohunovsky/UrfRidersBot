using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.ColorRole
{
    public interface IColorRoleService
    {
        Task SetColorRoleAsync(DiscordMember member, DiscordColor color);
        ValueTask<bool> RemoveColorRoleAsync(DiscordMember member);
    }
}