using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Persistence.DTOs;

namespace UrfRidersBot.Persistence.Repositories
{
    public class ColorRoleRepository : IColorRoleRepository
    {
        private readonly UrfRidersDbContext _context;

        public ColorRoleRepository(UrfRidersDbContext context)
        {
            _context = context;
        }
        
        public async Task<DiscordRole?> GetByMemberAsync(DiscordMember member)
        {
            var roleId = await _context.ColorRoles
                .AsNoTracking()
                .Where(x => x.GuildId == member.Guild.Id && x.UserId == member.Id)
                .Select(x => x.RoleId)
                .SingleOrDefaultAsync();

            if (roleId == default)
                return null;

            var guild = member.Guild;
            return guild.GetRole(roleId);
        }

        public async Task AddAsync(DiscordRole role, DiscordMember member)
        {
            await _context.ColorRoles.AddAsync(ColorRoleDTO.FromDiscord(role, member));
        }

        public void Remove(DiscordRole role, DiscordMember member)
        {
            _context.ColorRoles.Remove(ColorRoleDTO.FromDiscord(role, member));
        }
    }
}