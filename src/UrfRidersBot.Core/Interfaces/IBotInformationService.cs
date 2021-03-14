using System;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IBotInformationService
    {
        string Version { get; }
        TimeSpan Uptime { get; }

        public void SetStartTime(DateTimeOffset startTime);
    }
}