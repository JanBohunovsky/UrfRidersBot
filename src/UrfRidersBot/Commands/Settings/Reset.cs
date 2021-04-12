using System.Threading.Tasks;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Commands.Settings
{
    public class Reset : ICommand<SettingsGroup>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public bool Ephemeral => false;
        public string Name => "reset";
        public string Description => "Resets this server's settings back to default values.";

        public Reset(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var settings = await _unitOfWork.GuildSettings.GetAsync(context.Guild);

            if (settings is not null)
            {
                _unitOfWork.GuildSettings.Remove(settings);
                await _unitOfWork.CompleteAsync();
            }
            
            return CommandResult.Success($"All settings for {context.Guild.Name} has been reset.");
        }
    }
}