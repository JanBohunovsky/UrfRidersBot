using System;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IBotInformationService
    {
        TimeSpan Uptime { get; }

        public void SetStartTime(DateTimeOffset startTime);
    }
}