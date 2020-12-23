using Discord;

namespace UrfRidersBot.Library.Internal.Services
{
    internal class EmbedService : IEmbedService
    {
        private readonly BotConfiguration _botConfig;

        public EmbedService(BotConfiguration botConfig)
        {
            _botConfig = botConfig;
        }

        public EmbedBuilder Basic(string? description = null, string? title = null)
        {
            var builder = new EmbedBuilder()
                .WithColor(_botConfig.EmbedColor);

            if (description != null)
                builder.WithDescription(description);
            if (title != null)
                builder.WithTitle(title);

            return builder;
        }

        public EmbedBuilder Success(string? description = null, string title = "Success")
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithAuthor(title, "https://cdn.discordapp.com/attachments/717788228899307551/791314528280641546/icons8-checkmark-96.png");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }

        public EmbedBuilder Error(string? description = null, string? title = "Error")
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Gold)
                .WithAuthor(title, "https://cdn.discordapp.com/attachments/717788228899307551/791314543917006928/icons8-error-96.png");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }

        public EmbedBuilder CriticalError(string? description = null, string? title = "Critical Error")
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(title, "https://cdn.discordapp.com/attachments/717788228899307551/791314550561570876/icons8-high-priority-96.png");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }
    }
}