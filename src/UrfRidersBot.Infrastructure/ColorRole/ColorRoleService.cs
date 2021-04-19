using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.ColorRole;

namespace UrfRidersBot.Infrastructure.ColorRole
{
    internal class ColorRoleService : IColorRoleService
    {
        private readonly IColorRoleRepository _repository;

        public ColorRoleService(IColorRoleRepository repository)
        {
            _repository = repository;
        }
        
        public async Task SetColorRoleAsync(DiscordMember member, DiscordColor color)
        {
            var role = await _repository.GetByUserAsync(member);

            if (role != null)
            {
                await role.ModifyAsync(r => r.Color = color);
                return;
            }
            
            // Create color role and save it to the database
            role = await CreateColorRoleAsync(member, color);
            await member.GrantRoleAsync(role);
            
            await _repository.AddAsync(role, member);
        }

        public async ValueTask<bool> RemoveColorRoleAsync(DiscordMember member)
        {
            var role = await _repository.GetByUserAsync(member);

            if (role == null)
            {
                return false;
            }

            await _repository.RemoveAsync(role);
            
            await member.RevokeRoleAsync(role);
            await role.DeleteAsync();
            
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