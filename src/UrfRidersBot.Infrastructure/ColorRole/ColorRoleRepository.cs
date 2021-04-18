using System.Threading.Tasks;
using DSharpPlus.Entities;
using LiteDB.Async;
using UrfRidersBot.Core.ColorRole;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Infrastructure.ColorRole
{
    internal class ColorRoleRepository : LiteRepository<ColorRoleDTO>, IColorRoleRepository
    {
        private readonly DiscordGuild _guild;

        public ColorRoleRepository(ILiteDatabaseAsync db, DiscordGuild guild) : base(db, "color_roles")
        {
            _guild = guild;
        }
        
        public async ValueTask<DiscordRole?> GetByUserAsync(DiscordUser user)
        {
            var roleId = await Collection.Query()
                .Where(x => x.GuildId == _guild.Id && x.UserId == user.Id)
                .Select(x => x.RoleId)
                .SingleOrDefaultAsync();

            if (roleId == default)
            {
                return null;
            }

            return _guild.GetRole(roleId);
        }

        public async ValueTask<bool> AddAsync(DiscordRole role, DiscordUser user)
        {
            var collection = Collection;
            await collection.EnsureIndexAsync(x => x.GuildId);

            var dto = ColorRoleDTO.FromDiscord(role, _guild, user);
            
            // Users can only have one color role for each guild (and role can only have one user per guild)
            var exists = await collection.ExistsAsync(x => x.GuildId == dto.GuildId
                                                           && (x.RoleId == dto.RoleId || x.UserId == dto.UserId));

            if (exists)
            {
                return false;
            }

            await collection.InsertAsync(dto);
            return true;
        }

        public async Task RemoveAsync(DiscordRole role)
        {
            await Collection.DeleteManyAsync(x => x.RoleId == role.Id);
        }
    }
}