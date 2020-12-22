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
                .WithTitle($"{title} {_botConfig.Emotes.Ok}");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }

        public EmbedBuilder Error(string? description = null, string? title = "Error")
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Gold)
                .WithTitle($"{title} {_botConfig.Emotes.Error}");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }

        public EmbedBuilder CriticalError(string? description = null, string? title = "Critical Error")
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle($"{title} {_botConfig.Emotes.Critical}");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }
    }
}