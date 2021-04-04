using System;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Infrastructure.Common
{
    public class BotInformationService : IBotInformationService
    {
        private DateTimeOffset _startTime;
        
        public TimeSpan Uptime => DateTimeOffset.Now - _startTime;
        
        public void SetStartTime(DateTimeOffset startTime)
        {
            _startTime = startTime;
        }
    }
}