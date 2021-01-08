using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;

namespace UrfRidersBot
{
    public class UrfRidersHelpFormatter : DefaultHelpFormatter
    {
        public UrfRidersHelpFormatter(CommandContext ctx) : base(ctx)
        {
            // Replace the embed color with our own
            EmbedBuilder.WithColor(UrfRidersColor.Cyan);
        }
    }
}