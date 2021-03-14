using System;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
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