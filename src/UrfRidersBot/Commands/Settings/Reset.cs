using System.Threading.Tasks;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Settings;

namespace UrfRidersBot.Commands.Settings
{
    public class Reset : ICommand<SettingsGroup>
    {
        private readonly IGuildSettingsRepository _repository;
        
        public bool Ephemeral => false;
        public string Name => "reset";
        public string Description => "Resets this server's settings back to default values.";

        public Reset(IGuildSettingsRepository repository)
        {
            _repository = repository;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var settings = await _repository.GetAsync();

            if (settings is not null)
            {
                await _repository.RemoveAsync();
            }
            
            return CommandResult.Success($"All settings for {context.Guild.Name} has been reset.");
        }
    }
}