using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
{
    public class VersionService : IVersionService
    {
        // This may not be the best solution but I can't bother figuring out where to put the Version tag.
        // If I'm gonna need automatic version bumping in builds then I will rework this, but now it's not worth it.
        private readonly string _botVersion = "2.0.0";
        private readonly IHostEnvironment _environment;

        public VersionService(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public string BotVersion => _environment.IsDevelopment() ? $"{_botVersion}-dev" : _botVersion;
    }
}