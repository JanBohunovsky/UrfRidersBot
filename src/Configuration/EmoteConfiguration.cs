using System;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;

namespace UrfRidersBot
{
    public class EmoteConfiguration : BaseConfiguration
    {
        private readonly DiscordClient _client;
        private readonly IConfigurationSection _rawEmotes;
        
        // System.Lazy seems like the best solutions here.
        // ctor: Doesn't work because class gets initialized before the client even starts.
        // DiscordClient.Ready event: Doesn't work because the client didn't download any guild data.
        // One more suggestion is to use some kind of initialization method that will be called on BaseCommandModule.BeforeExecutionAsync,
        // but this leaves the work on the module which I don't want.
        private readonly Lazy<DiscordEmoji> _yes;
        private readonly Lazy<DiscordEmoji> _no;

        public DiscordEmoji Yes => _yes.Value;
        public DiscordEmoji No => _no.Value;

        public EmoteConfiguration(DiscordClient client, IConfiguration configuration)
        {
            _client = client;
            _rawEmotes = configuration.GetSection("Emotes");

            _yes = CreateLazyEmote(nameof(Yes), ":white_check_mark:");
            _no = CreateLazyEmote(nameof(No), ":x:");
        }

        private Lazy<DiscordEmoji> CreateLazyEmote(string propertyName, string defaultEmoteName) =>
            new(() => ParseEmote(_client, _rawEmotes[propertyName], defaultEmoteName));

        /// <summary>
        /// Creates a new <see cref="DiscordEmoji"/> from either emote name (e.g. :thinking:) or a guild emote ID.
        /// <para>
        /// If <see cref="rawValue"/> is null, then <see cref="defaultEmoteName"/> is used.
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="rawValue">Raw value from configuration, can be <see cref="string"/> or <see cref="ulong"/>.</param>
        /// <param name="defaultEmoteName">Fallback emote name if <see cref="rawValue"/> is null.</param>
        private static DiscordEmoji ParseEmote(DiscordClient client, string? rawValue, string defaultEmoteName)
        {
            if (rawValue == null)
                return DiscordEmoji.FromName(client, defaultEmoteName);
            
            if (ulong.TryParse(rawValue, out ulong emoteId))
            {
                return DiscordEmoji.FromGuildEmote(client, emoteId);
            }

            return DiscordEmoji.FromName(client, rawValue);
        }
    }
}