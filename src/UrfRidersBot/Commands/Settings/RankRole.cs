using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Attributes;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Commands.Settings
{
    public class RankRole : ICommand<SettingsGroup>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public bool Ephemeral => false;
        public string Name => "rank-role";
        public string Description => "Set custom role for specific guild rank.";
        
        [Parameter("guild-rank", "Target guild rank to set custom role for.")]
        public GuildRankRole GuildRank { get; set; }
        
        [Parameter("role", "Target role for selected guild rank. Leave empty to reset back to default.", true)]
        public DiscordRole? Role { get; set; }

        public RankRole(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var settings = await _unitOfWork.GuildSettings.GetOrCreateAsync(context.Guild);

            switch (GuildRank)
            {
                case GuildRankRole.Member:
                    settings.MemberRoleId = Role?.Id;
                    break;
                case GuildRankRole.Moderator:
                    settings.ModeratorRoleId = Role?.Id;
                    break;
                case GuildRankRole.Admin:
                    settings.AdminRoleId = Role?.Id;
                    break;
            }

            await _unitOfWork.CompleteAsync();

            return CommandResult.Success($"{GuildRank.ToString()} role has been set to {Role?.Mention ?? "default"}.");
        }

        public enum GuildRankRole
        {
            Member,
            Moderator,
            Admin
        }
    }
}