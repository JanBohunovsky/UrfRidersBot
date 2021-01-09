using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;

namespace UrfRidersBot
{
    public class EmoteConfiguration : BaseConfiguration, IOnReadyService
    {
        private readonly IConfigurationSection _rawEmotes;

        public DiscordEmoji Yes { get; private set; }
        public DiscordEmoji No { get; private set; }
        public DiscordEmoji SkipLeft { get; private set; }
        public DiscordEmoji Left { get; private set; }
        public DiscordEmoji Right { get; private set; }
        public DiscordEmoji SkipRight { get; private set; }
        public DiscordEmoji Stop { get; private set; }

        public EmoteConfiguration(IConfiguration configuration)
        {
            _rawEmotes = configuration.GetSection("Emotes");
            
            // Default emotes
            Yes = DiscordEmoji.FromUnicode("✅");
            No = DiscordEmoji.FromUnicode("❌");
            SkipLeft = DiscordEmoji.FromUnicode("⏮");
            Left = DiscordEmoji.FromUnicode("◀");
            Right = DiscordEmoji.FromUnicode("▶");
            SkipRight = DiscordEmoji.FromUnicode("⏭");
            Stop = DiscordEmoji.FromUnicode("⏹");
        }

        public Task OnReady(DiscordClient client)
        {
            // Load custom emotes
            Yes = ParseEmote(client, _rawEmotes[nameof(Yes)]) ?? Yes;
            No = ParseEmote(client, _rawEmotes[nameof(No)]) ?? No;
            SkipLeft = ParseEmote(client, _rawEmotes[nameof(Right)]) ?? SkipLeft;
            Left = ParseEmote(client, _rawEmotes[nameof(Right)]) ?? Left;
            Right = ParseEmote(client, _rawEmotes[nameof(Right)]) ?? Right;
            SkipRight = ParseEmote(client, _rawEmotes[nameof(Right)]) ?? SkipRight;
            Stop = ParseEmote(client, _rawEmotes[nameof(Right)]) ?? Stop;
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a new <see cref="DiscordEmoji"/> from either emote name (e.g. :thinking:) or a guild emote ID.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="rawValue">Raw value from configuration, can be <see cref="string"/> or <see cref="ulong"/>.</param>
        private static DiscordEmoji? ParseEmote(DiscordClient client, string? rawValue)
        {
            // If there is no emote configured -> return null
            if (rawValue == null)
                return null;
            
            // If there is -> try to parse it (either by id or name), this can fail (key not found or name not found),
            // but that's ok, I want that.
            if (ulong.TryParse(rawValue, out ulong emoteId))
            {
                return DiscordEmoji.FromGuildEmote(client, emoteId);
            }

            return DiscordEmoji.FromName(client, rawValue);
        }
    }
}