using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Infrastructure.Common
{
    internal class DiscordBot : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly IBotInformationService _botInfo;

        public DiscordBot(DiscordClient client, IBotInformationService botInfo)
        {
            _client = client;
            _botInfo = botInfo;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _botInfo.SetStartTime(DateTimeOffset.Now);
            await _client.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
        }
    }
}