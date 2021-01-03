using Discord;
using Discord.WebSocket;

namespace UrfRidersBot
{
    public class EmbedService
    {
        private readonly BotConfiguration _botConfig;
        private readonly DiscordSocketClient _client;

        public EmbedService(BotConfiguration botConfig, DiscordSocketClient client)
        {
            _botConfig = botConfig;
            _client = client;
        }

        public EmbedBuilder CreateBasic(string? description = null, string? title = null)
        {
            return new EmbedBuilder
            {
                Color = _botConfig.Color,
                Title = title,
                Description = description,
            };
        }

        public EmbedBuilder CreateBotInfo(string? nameSuffix = null)
        {
            return new EmbedBuilder
            {
                Color = _botConfig.Color,
                Author = new EmbedAuthorBuilder
                {
                    Name = nameSuffix == null ? _botConfig.Name : $"{_botConfig.Name} {nameSuffix}",
                    IconUrl = _client.CurrentUser.GetAvatarUrl(),
                },
            };
        }

        public EmbedBuilder CreateSuccess(string? description = null, string title = "Success")
        {
            return new EmbedBuilder
            {
                Color = Color.Green,
                Author = new EmbedAuthorBuilder
                {
                    Name = title,
                    IconUrl = "https://cdn.discordapp.com/attachments/717788228899307551/791314528280641546/icons8-checkmark-96.png",
                },
                Description = description,
            };
        }

        public EmbedBuilder CreateError(string? description = null, string? title = "Error")
        {
            return new EmbedBuilder
            {
                Color = Color.Gold,
                Author = new EmbedAuthorBuilder
                {
                    Name = title,
                    IconUrl = "https://cdn.discordapp.com/attachments/717788228899307551/791314543917006928/icons8-error-96.png",
                },
                Description = description,
            };
        }

        public EmbedBuilder CreateCriticalError(string? description = null, string? title = "Critical Error")
        {
            return new EmbedBuilder
            {
                Color = Color.Red,
                Author = new EmbedAuthorBuilder
                {
                    Name = title,
                    IconUrl = "https://cdn.discordapp.com/attachments/717788228899307551/791314550561570876/icons8-high-priority-96.png",
                },
                Description = description,
            };
        }
    }
}