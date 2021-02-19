using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using UrfRidersBot.Core;

namespace UrfRidersBot.Infrastructure.Commands
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