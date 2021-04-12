using System.Threading.Tasks;
using UrfRidersBot.Core.ColorRole;
using UrfRidersBot.Core.Commands;

namespace UrfRidersBot.Commands.Color
{
    public class Reset : ICommand<ColorGroup>
    {
        private readonly IColorRoleService _service;

        public bool Ephemeral => true;
        public string Name => "reset";
        public string Description => "Remove your custom role and reset your color.";

        public Reset(IColorRoleService service)
        {
            _service = service;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            await _service.RemoveColorRoleAsync(context.Member);
            
            return CommandResult.Success("Custom role has been removed.");
        }
    }
}