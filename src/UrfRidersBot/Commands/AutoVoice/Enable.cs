using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Core.Commands;

namespace UrfRidersBot.Commands.AutoVoice
{
    public class Enable : ICommand<AutoVoiceGroup>
    {
        private readonly IAutoVoiceSettingsRepository _repository;
        private readonly IAutoVoiceService _service;

        public bool Ephemeral => true;
        public string Name => "enable";
        public string Description => "Enables the module and creates a new voice channel.";

        [Parameter("category", "Under which category should the voice channel be created.", true)]
        public DiscordChannel? Category { get; set; }

        public Enable(IAutoVoiceSettingsRepository repository, IAutoVoiceService service)
        {
            _repository = repository;
            _service = service;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            if (Category is not null && !Category.IsCategory)
            {
                return CommandResult.InvalidParameter("Channel must be a category.");
            }

            var settings = await _repository.GetOrCreateAsync();
            if (settings.ChannelCreator is not null)
            {
                return CommandResult.InvalidOperation("Auto Voice is already enabled on this server.");
            }

            var voiceChannelCreator = await _service.CreateAsync(context.Guild, Category, settings.Bitrate);

            settings.ChannelCreator = voiceChannelCreator;
            await _repository.SaveAsync(settings);

            var sb = new StringBuilder();
            sb.AppendLine("Auto Voice has been enabled.");
            sb.Append($"Created new voice channel `{voiceChannelCreator.Name}`");

            if (voiceChannelCreator.Parent != null)
            {
                sb.Append($" under category `{voiceChannelCreator.Parent.Name}`");
            }

            sb.AppendLine(".");

            return CommandResult.Success(sb.ToString());
        }
    }
}