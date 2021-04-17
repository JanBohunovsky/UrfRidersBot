using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.ColorRole;

namespace UrfRidersBot.Infrastructure.ColorRole
{
    internal class ColorRoleService : IColorRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ColorRoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task SetColorRoleAsync(DiscordMember member, DiscordColor color)
        {
            var role = await _unitOfWork.ColorRoles.GetByUser(member);

            if (role != null)
            {
                await role.ModifyAsync(r => r.Color = color);
                return;
            }
            
            role = await CreateColorRoleAsync(member, color);

            await member.GrantRoleAsync(role);
            
            await _unitOfWork.ColorRoles.Add(role, member);
            await _unitOfWork.CompleteAsync();
        }

        public async ValueTask<bool> RemoveColorRoleAsync(DiscordMember member)
        {
            var role = await _unitOfWork.ColorRoles.GetByUser(member);

            if (role == null)
            {
                return false;
            }

            _unitOfWork.ColorRoles.Remove(role, member);
            
            await member.RevokeRoleAsync(role);
            await role.DeleteAsync();
            
            await _unitOfWork.CompleteAsync();
            
            return true;
        }

        private async ValueTask<DiscordRole> CreateColorRoleAsync(DiscordMember member, DiscordColor color)
        {
            var guild = member.Guild;
            var role = await guild.CreateRoleAsync(member.Username, color: color);

            var position = GetHighestRolePosition(guild);
            await role.ModifyPositionAsync(position);

            return role;
        }

        private int GetHighestRolePosition(DiscordGuild guild)
        {
            var botRole = guild.CurrentMember.Roles
                .OrderByDescending(r => r.Position)
                .First();

            return botRole.Position - 1;
        }
        
    }
}