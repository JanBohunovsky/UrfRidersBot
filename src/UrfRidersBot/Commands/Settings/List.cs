using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Commands.Settings
{
    public class List : ICommand<SettingsGroup>
    {
        private const string NullText = "*null*";
        private readonly IUnitOfWork _unitOfWork;
        
        public bool Ephemeral => false;
        public string Name => "list";
        public string Description => "Shows current settings for this server.";

        public List(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Cyan,
                Title = "Settings",
                Description = $"Current settings for {context.Guild.Name}.",
            };

            var settings = await _unitOfWork.GuildSettings.GetOrCreateAsync(context.Guild);

            embed
                .AddField(
                    "Member role",
                    settings.GetMemberRole(context.Guild)?.Mention ?? NullText,
                    true)
                .AddField(
                    "Moderator role",
                    settings.GetModeratorRole(context.Guild)?.Mention ?? NullText,
                    true)
                .AddField(
                    "Admin role",
                    settings.GetAdminRole(context.Guild)?.Mention ?? NullText,
                    true);
            
            await context.RespondAsync(embed);
            
            return CommandResult.NoAction;
        }
    }
}