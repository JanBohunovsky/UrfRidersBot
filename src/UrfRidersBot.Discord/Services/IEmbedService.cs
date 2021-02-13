using DSharpPlus.Entities;

namespace UrfRidersBot.Discord
{
    public interface IEmbedService
    {
        DiscordEmbedBuilder CreateBotInfo(string? nameSuffix = null);
        DiscordEmbedBuilder CreateSuccess(string? description = null, string title = "Success");
        DiscordEmbedBuilder CreateError(string? description = null, string? title = "Error");
        DiscordEmbedBuilder CreateCriticalError(string? description = null, string? title = "Critical Error");
    }
}