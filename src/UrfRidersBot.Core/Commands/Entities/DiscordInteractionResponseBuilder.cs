using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace UrfRidersBot.Core.Commands.Entities
{
    // Inspired by:
    // https://github.com/IDoEverything/DSharpPlusSlashCommands
    public class DiscordInteractionResponseBuilder
    {
        [JsonProperty("tts", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTTS { get; set; }
        
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string? Content { get; set; }
        
        [JsonProperty("embeds", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<DiscordEmbed> Embeds { get; private set; } = new List<DiscordEmbed>();
        
        [JsonProperty("mentions", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<IMention> Mentions { get; set; } = new List<IMention>();
        
        [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
        public InteractionFlags? Flags { get; set; }

        public DiscordInteractionResponseBuilder WithEmbeds(params DiscordEmbed[] embeds)
        {
            var list = Embeds.ToList();
            list.AddRange(embeds);
            Embeds = list;
            return this;
        }

        public DiscordInteractionResponseBuilder WithContent(string content)
        {
            Content = content;
            return this;
        }

        public DiscordInteractionResponseBuilder WithTTS(bool tts)
        {
            IsTTS = tts;
            return this;
        }

        public DiscordInteractionResponseBuilder WithMention(IMention mention)
        {
            var list = Mentions.ToList();
            list.Add(mention);
            Mentions = list;
            return this;
        }

        public DiscordInteractionResponseBuilder WithMentions(IEnumerable<IMention> mentions)
        {
            var list = Mentions.ToList();
            list.AddRange(mentions);
            Mentions = list;
            return this;
        }

        public DiscordInteractionResponseBuilder WithFlags(InteractionFlags flags)
        {
            Flags = flags;
            return this;
        }
    }
}