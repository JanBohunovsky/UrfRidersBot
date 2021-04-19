using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.ColorRole;
using UrfRidersBot.Core.Commands;

namespace UrfRidersBot.Commands.Color
{
    public class Set : ICommand<ColorGroup>
    {
        private readonly IColorRoleService _service;

        public bool Ephemeral => true;
        public string Name => "set";
        public string Description => "Give yourself a role with custom color.";

        [Parameter("color", "Color you wish to use in HEX, for example: `#ff8000`.")]
        public string ColorHex { get; set; } = "";

        public Set(IColorRoleService service)
        {
            _service = service;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            DiscordColor color;

            try
            {
                color = new DiscordColor(ColorHex);
            }
            catch (ArgumentException e)
            {
                return CommandResult.InvalidParameter(e.Message);
            }

            if (color.Value == DiscordColor.None.Value)
            {
                // If user entered pure black, set it to near black because pure black is treated as transparent.
                color = DiscordColor.Black;
            }
            
            await _service.SetColorRoleAsync(context.Member, color.Value);

            return CommandResult.Success($"Your color has been set to {ColorHex}.");
        }
    }
}