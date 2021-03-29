using System.Collections.Generic;
using System.Threading.Tasks;
using UrfRidersBot.Core.Commands.Built;
using UrfRidersBot.Core.Commands.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IInteractionService
    {
        Task CreateResponseAsync(
            ulong interactionId,
            string token,
            DiscordInteractionResponseType type, 
            DiscordInteractionResponseBuilder? builder = null);

        Task EditResponseAsync(string token, DiscordInteractionResponseBuilder builder);

        Task DeleteResponseAsync(string token);

        /// <summary>
        /// Registers slash commands to discord either globally or for a specific guild.
        /// </summary>
        /// <param name="commands">Slash commands you wish to register.</param>
        /// <param name="guildId">Guild to register commands for, leave empty for global.</param>
        Task RegisterCommandsAsync(IEnumerable<SlashCommand> commands, ulong? guildId = null);
    }
}