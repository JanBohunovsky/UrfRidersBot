using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IColorRoleService
    {
        Task SetColorRoleAsync(DiscordMember member, DiscordColor color);
        ValueTask<bool> RemoveColorRoleAsync(DiscordMember member);
    }
}