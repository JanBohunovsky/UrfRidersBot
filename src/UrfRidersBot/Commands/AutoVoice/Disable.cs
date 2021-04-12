using System.Text;
using System.Threading.Tasks;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Commands.AutoVoice
{
    public class Disable : ICommand<AutoVoiceGroup>
    {
        private readonly IUnitOfWork _unitOfWork;

        public bool Ephemeral => true;
        public string Name => "disable";
        public string Description => "Disables the module and deletes all the voice channels created by this module.";

        public Disable(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var settings = await _unitOfWork.AutoVoiceSettings.GetAsync(context.Guild);
            if (settings?.ChannelCreatorId is null)
            {
                return CommandResult.InvalidOperation("Auto Voice is already disabled on this server.");
            }

            foreach (var voiceChannel in settings.GetVoiceChannels(context.Guild))
            {
                await voiceChannel.DeleteAsync();
            }
            var count = settings.RemoveAllChannels();

            var channelCreator = settings.GetChannelCreator(context.Guild);
            if (channelCreator is not null)
            {
                await channelCreator.DeleteAsync();
                count++;
            }

            settings.ChannelCreatorId = null;
            await _unitOfWork.CompleteAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Auto Voice has been disabled.");
            sb.AppendLine($"Deleted {count} channel(s).");

            return CommandResult.Success(sb.ToString());
        }
    }
}