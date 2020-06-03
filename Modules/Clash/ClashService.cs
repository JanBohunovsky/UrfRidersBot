using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using System.Threading.Tasks;

namespace UrfRiders.Modules.Clash
{
    public class ClashService
    {
        private readonly DiscordSocketClient _client;
        private readonly LiteDatabase _database;
        private readonly ILogger _logger;
        private readonly RiotApi _riotApi;

        public ClashService(DiscordSocketClient client, LiteDatabase database, ILogger<ClashService> logger, RiotApi riotApi)
        {
            _client = client;
            _database = database;
            _logger = logger;
            _riotApi = riotApi;
        }

        public async Task GetTournaments()
        {

        }
    }
}