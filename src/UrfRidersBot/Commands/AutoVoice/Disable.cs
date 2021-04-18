using System.Text;
using System.Threading.Tasks;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Core.Commands;

namespace UrfRidersBot.Commands.AutoVoice
{
    public class Disable : ICommand<AutoVoiceGroup>
    {
        private readonly IAutoVoiceSettingsRepository _repository;

        public bool Ephemeral => true;
        public string Name => "disable";
        public string Description => "Disables the module and deletes all the voice channels created by this module.";

        public Disable(IAutoVoiceSettingsRepository repository)
        {
            _repository = repository;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var settings = await _repository.GetAsync();
            if (settings?.ChannelCreator is null)
            {
                return CommandResult.InvalidOperation("Auto Voice is already disabled on this server.");
            }

            foreach (var voiceChannel in settings.VoiceChannels)
            {
                await voiceChannel.DeleteAsync();
            }
            var count = settings.RemoveAllChannels();

            if (settings.ChannelCreator is not null)
            {
                await settings.ChannelCreator.DeleteAsync();
                settings.ChannelCreator = null;
                count++;
            }

            await _repository.SaveAsync(settings);

            var sb = new StringBuilder();
            sb.AppendLine("Auto Voice has been disabled.");
            sb.AppendLine($"Deleted {count} channel(s).");

            return CommandResult.Success(sb.ToString());
        }
    }
}