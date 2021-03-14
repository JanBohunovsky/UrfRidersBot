using System;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
{
    public class BotInformationService : IBotInformationService
    {
        // This may not be the best solution but I can't bother figuring out where to put the Version tag.
        // If I'm gonna need automatic version bumping in builds then I will rework this, but now it's not worth it.
        private readonly string _version = "2.0.0";
        private readonly IHostEnvironment _environment;

        private DateTimeOffset _startTime;

        public BotInformationService(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public string Version => _environment.IsDevelopment() ? $"{_version}-dev" : _version;
        public TimeSpan Uptime => DateTimeOffset.Now - _startTime;
        
        public void SetStartTime(DateTimeOffset startTime)
        {
            _startTime = startTime;
        }
    }
}