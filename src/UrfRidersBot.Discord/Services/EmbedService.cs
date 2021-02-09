using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Discord
{
    internal class EmbedService : IEmbedService
    {
        private readonly DiscordClient _client;

        public EmbedService(DiscordClient client)
        {
            _client = client;
        }

        public DiscordEmbedBuilder CreateBotInfo(string? nameSuffix = null)
        {
            var name = "Official UrfRiders Bot";
            if (nameSuffix != null)
            {
                name = $"{name} {nameSuffix}";
            }
            
            return new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Cyan,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = name,
                    IconUrl = _client.CurrentUser.GetAvatarUrl(ImageFormat.Auto),
                },
            };
        }

        public DiscordEmbedBuilder CreateSuccess(string? description = null, string title = "Success")
        {
            return new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Green,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = title,
                    IconUrl = UrfRidersIcon.Checkmark,
                },
                Description = description,
            };
        }

        public DiscordEmbedBuilder CreateError(string? description = null, string? title = "Error")
        {
            return new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Yellow,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = title,
                    IconUrl = UrfRidersIcon.Error,
                },
                Description = description,
            };
        }

        public DiscordEmbedBuilder CreateCriticalError(string? description = null, string? title = "Critical Error")
        {
            return new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Red,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = title,
                    IconUrl = UrfRidersIcon.HighPriority,
                },
                Description = description,
            };
        }
    }
}