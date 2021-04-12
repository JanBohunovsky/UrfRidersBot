using UrfRidersBot.Core.Commands;
using UrfRidersBot.Infrastructure.Commands.Checks;

namespace UrfRidersBot.Commands.AutoVoice
{
    [RequireGuildRank(GuildRank.Admin)]
    public class AutoVoiceGroup : ICommandGroup
    {
        public string Name => "auto-voice";
        public string Description => "Automatically create new voice channel whenever all are taken.";
    }
}