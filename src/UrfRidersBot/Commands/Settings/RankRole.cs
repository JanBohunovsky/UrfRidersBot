using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Settings;

namespace UrfRidersBot.Commands.Settings
{
    public class RankRole : ICommand<SettingsGroup>
    {
        private readonly IGuildSettingsRepository _repository;
        
        public bool Ephemeral => false;
        public string Name => "rank-role";
        public string Description => "Set custom role for specific guild rank.";
        
        [Parameter("guild-rank", "Target guild rank to set custom role for.")]
        public GuildRankRole GuildRank { get; set; }
        
        [Parameter("role", "Target role for selected guild rank. Leave empty to reset back to default.", true)]
        public DiscordRole? Role { get; set; }

        public RankRole(IGuildSettingsRepository repository)
        {
            _repository = repository;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var settings = await _repository.GetOrCreateAsync();

            switch (GuildRank)
            {
                case GuildRankRole.Member:
                    settings.MemberRole = Role;
                    break;
                case GuildRankRole.Moderator:
                    settings.ModeratorRole = Role;
                    break;
                case GuildRankRole.Admin:
                    settings.AdminRole = Role;
                    break;
            }

            await _repository.SaveAsync(settings);

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