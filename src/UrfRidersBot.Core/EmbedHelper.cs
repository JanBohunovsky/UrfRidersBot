using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core
{
    public static class EmbedHelper
    {
        public static DiscordEmbedBuilder CreateBotInfo(DiscordClient client, string? nameSuffix = null)
        {
            var name = client.CurrentApplication.Name;
            if (nameSuffix != null)
            {
                name = $"{name} {nameSuffix}";
            }
            
            return new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Theme,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = name,
                    IconUrl = client.CurrentApplication.Icon,
                },
            };
        }

        public static DiscordEmbedBuilder CreateSuccess(string? description = null, string title = "Success")
        {
            return new()
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

        public static DiscordEmbedBuilder CreateError(string? description = null, string title = "Error")
        {
            return new()
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

        public static DiscordEmbedBuilder CreateCriticalError(string? description = null, string title = "Critical Error")
        {
            return new()
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

        public static DiscordEmbedBuilder CreateUnavailable(string? description = null, string title = "Unavailable")
        {
            return new()
            {
                Color = UrfRidersColor.Red,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = title,
                    IconUrl = UrfRidersIcon.Unavailable,
                },
                Description = description
            };
        }
    }
}