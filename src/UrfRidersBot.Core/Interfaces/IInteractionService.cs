using System.Threading.Tasks;
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
    }
}