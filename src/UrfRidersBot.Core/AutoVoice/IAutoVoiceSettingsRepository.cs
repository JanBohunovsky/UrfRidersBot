using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.AutoVoice
{
    public interface IAutoVoiceSettingsRepository : IRepository
    {
        AutoVoiceSettings? Get();
        AutoVoiceSettings GetOrCreate();
        void Save(AutoVoiceSettings settings);
        void Remove();
    }
}