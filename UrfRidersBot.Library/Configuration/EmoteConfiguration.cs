using Discord;
using Microsoft.Extensions.Configuration;

namespace UrfRidersBot.Library
{
    public class EmoteConfiguration
    {
        public IEmote Ok { get; set; }
        public IEmote Cancel { get; set; }
        public IEmote Checkmark { get; set; }
        public IEmote Cross { get; set; }
        public IEmote Yes { get; set; }
        public IEmote No { get; set; }
        public IEmote ThumbsUp { get; set; }
        public IEmote ThumbsDown { get; set; }
        
        public IEmote Error { get; set; }
        public IEmote Critical { get; set; }

        public IEmote First { get; set; }
        public IEmote Last { get; set; }
        public IEmote Previous { get; set; }
        public IEmote Next { get; set; }
        public IEmote Stop { get; set; }

        public EmoteConfiguration(IConfiguration configuration)
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