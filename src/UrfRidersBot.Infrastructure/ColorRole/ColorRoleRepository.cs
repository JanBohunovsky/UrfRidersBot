using DSharpPlus.Entities;
using LiteDB;
using UrfRidersBot.Core.ColorRole;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Infrastructure.ColorRole
{
    internal class ColorRoleRepository : LiteRepository<ColorRoleDTO>, IColorRoleRepository
    {
        private readonly DiscordGuild _guild;

        public ColorRoleRepository(LiteDatabase db, DiscordGuild guild) : base(db, "color_roles")
        {
            _guild = guild;
        }
        
        public DiscordRole? GetByUser(DiscordUser user)
        {
            var roleId = Collection.Query()
                .Where(x => x.GuildId == _guild.Id && x.UserId == user.Id)
                .Select(x => x.RoleId)
                .SingleOrDefault();

            if (roleId == default)
            {
                return null;
            }

            return _guild.GetRole(roleId);
        }

        public bool Add(DiscordRole role, DiscordUser user)
        {
            var collection = Collection;
            collection.EnsureIndex(x => x.GuildId);

            var dto = ColorRoleDTO.FromDiscord(role, _guild, user);
            
            // Users can only have one color role for each guild (and role can only have one user per guild)
            var exists = collection.Exists(x => x.GuildId == dto.GuildId
                                                && (x.RoleId == dto.RoleId || x.UserId == dto.UserId));

            if (exists)
            {
                return false;
            }

            collection.Insert(dto);
            return true;
        }

        public void Remove(DiscordRole role)
        {
            Collection.DeleteMany(x => x.RoleId == role.Id);
        }
    }
}