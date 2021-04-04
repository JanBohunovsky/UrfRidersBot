using System;

namespace UrfRidersBot.Core.Common
{
    public interface IBotInformationService
    {
        TimeSpan Uptime { get; }

        public void SetStartTime(DateTimeOffset startTime);
    }
}