using Discord;
using Microsoft.Extensions.Configuration;

namespace UrfRidersBot
{
    public class EmoteConfiguration : BaseConfiguration
    {
        public IEmote Ok { get; }
        public IEmote Cancel { get; }
        public IEmote Checkmark { get; }
        public IEmote Cross { get; }
        public IEmote Yes { get; }
        public IEmote No { get; }
        public IEmote ThumbsUp { get; }
        public IEmote ThumbsDown { get; }
        
        public IEmote Error { get; }
        public IEmote Critical { get; }

        public IEmote First { get; }
        public IEmote Last { get; }
        public IEmote Previous { get; }
        public IEmote Next { get; }
        public IEmote Stop { get; }

        public EmoteConfiguration(IConfiguration configuration) : base(configuration, "Bot:Emotes")
        {
            var emotes = configuration.GetSection("Bot:Emotes");
            Ok = (emotes[nameof(Ok)] ?? "✅").ToEmote();
            Cancel = (emotes[nameof(Cancel)] ?? "❌").ToEmote();
            Checkmark = (emotes[nameof(Checkmark)] ?? "✅").ToEmote();
            Cross = (emotes[nameof(Cross)] ?? "❌").ToEmote();
            Yes = (emotes[nameof(Yes)] ?? "✅").ToEmote();
            No = (emotes[nameof(No)] ?? "❌").ToEmote();
            ThumbsUp = (emotes[nameof(ThumbsUp)] ?? "👍").ToEmote();
            ThumbsDown = (emotes[nameof(ThumbsDown)] ?? "👎").ToEmote();

            Error = (emotes[nameof(Error)] ?? "⚠").ToEmote();
            Critical = (emotes[nameof(Critical)] ?? "❌").ToEmote();

            First = (emotes[nameof(First)] ?? "⏮").ToEmote();
            Last = (emotes[nameof(Last)] ?? "⏭").ToEmote();
            Previous = (emotes[nameof(Previous)] ?? "◀").ToEmote();
            Next = (emotes[nameof(Next)] ?? "▶").ToEmote();
            Stop = (emotes[nameof(Stop)] ?? "⏹").ToEmote();
        }
    }
}