using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Settings;

namespace UrfRidersBot.Commands.Settings
{
    public class List : ICommand<SettingsGroup>
    {
        private const string NullText = "*null*";
        private readonly IGuildSettingsRepository _repository;
        
        public bool Ephemeral => false;
        public string Name => "list";
        public string Description => "Shows current settings for this server.";

        public List(IGuildSettingsRepository repository)
        {
            _repository = repository;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Cyan,
                Title = "Settings",
                Description = $"Current settings for {context.Guild.Name}.",
            };

            var settings = await _repository.GetOrCreateAsync();

            embed
                .AddField(
                    "Member role",
                    settings.MemberRole?.Mention ?? NullText,
                    true)
                .AddField(
                    "Moderator role",
                    settings.ModeratorRole?.Mention ?? NullText,
                    true)
                .AddField(
                    "Admin role",
                    settings.AdminRole?.Mention ?? NullText,
                    true);
            
            await context.RespondAsync(embed);
            
            return CommandResult.NoAction;
        }
    }
}