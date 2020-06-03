using System;
using System.Linq;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using System.Threading.Tasks;
using MingweiSamuel.Camille.Enums;

namespace UrfRiders.Modules.Clash
{
    public class ClashService
    {
        private readonly DiscordSocketClient _client;
        private readonly LiteDatabase _database;
        private readonly ILogger _logger;
        private readonly RiotApi _riotApi;

        private bool _init;
        private ClashTournamentData[] _tournaments;

        public ClashService(DiscordSocketClient client, LiteDatabase database, ILogger<ClashService> logger, RiotApi riotApi)
        {
            _client = client;
            _database = database;
            _logger = logger;
            _riotApi = riotApi;

            // Run periodic update when the client is ready
            _client.Ready += () =>
            {
                if (!_init)
                {
                    Task.Run(PeriodicUpdate);
                    _init = true;
                }
                return Task.CompletedTask;
            };
        }

        public async Task<ClashTournamentData[]> GetTournaments()
        {
            return _tournaments ??= await DownloadTournaments();
        }

        private async Task<ClashTournamentData[]> DownloadTournaments()
        {
            var data = await _riotApi.ClashV1.GetTournamentsAsync(Region.EUNE);
            return data.Select(ClashTournamentData.Parse).OrderBy(d => d.RegistrationTime).ToArray();
        }

        private async Task PeriodicUpdate()
        {
            while (true)
            {
                var target = DateTimeOffset.Now.AddHours(1);

                // Update
                try
                {
                    _tournaments = await DownloadTournaments();
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Could not fetch upcoming tournaments.");
                }

                //TODO: Check if each server has seen these tournaments by comparing TournamentIds

                // Wait
                await Task.Delay(target - DateTimeOffset.Now);
            }
        }
    }
}