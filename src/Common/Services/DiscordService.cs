using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Hosting;

namespace UrfRidersBot.Common.Services
{
    public class DiscordService : IHostedService
    {
        private readonly DiscordClient _client;

        public DiscordService(DiscordClient client)
        {
            _client = client;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
        }
    }
}