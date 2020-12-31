using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace UrfRidersBot.Library
{
    internal class DiscordHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discord;
        private readonly SecretsConfiguration _secrets;

        public DiscordHostedService(DiscordSocketClient discord, SecretsConfiguration secrets)
        {
            _discord = discord;
            _secrets = secrets;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _discord.LoginAsync(TokenType.Bot, _secrets.DiscordToken);
            await _discord.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discord.LogoutAsync();
            await _discord.StopAsync();
        }
    }
}