using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Attributes;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Commands.AutoVoice
{
    public class Bitrate : ICommand<AutoVoiceGroup>
    {
        private readonly IUnitOfWork _unitOfWork;

        public bool Ephemeral => true;
        public string Name => "bitrate";
        public string Description => "Change the bitrate of Auto Voice channels.";
        
        [Parameter("value", "The target bitrate in Kbps.")]
        public long Value { get; set; }

        public Bitrate(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            const int minBitrate = 8;
            var maxBitrate = context.Guild.PremiumTier switch
            {
                PremiumTier.Tier_1  => 128,
                PremiumTier.Tier_2  => 256,
                PremiumTier.Tier_3  => 384,
                PremiumTier.Unknown => 384, // Handle unknown as a new tier above 3
                _                   => 96
            };

            if (Value < minBitrate || Value > maxBitrate)
            {
                return CommandResult.InvalidParameter($"Bitrate must be between {minBitrate} and {maxBitrate} Kbps.");
            }

            var settings = await _unitOfWork.AutoVoiceSettings.GetOrCreateAsync(context.Guild);
            settings.Bitrate = (int)Value;
            await _unitOfWork.CompleteAsync();

            var sb = new StringBuilder();
            sb.AppendLine($"Auto Voice channel's bitrate has been set to {Value} Kbps.");
            sb.AppendLine("Existing Auto Voice channels won't be updated.");

            return CommandResult.Success(sb.ToString());
        }
    }
}