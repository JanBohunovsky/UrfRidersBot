using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.AspNetCore.Mvc;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.WebAPI.Controllers
{
    // Temporary: Just for testing the API
    [Route("api/[controller]")]
    [ApiController]
    public class AutoVoiceController : Controller
    {
        public record AutoVoiceChannel(ulong ChannelId, string ChannelName, int UserCount);

        public record AutoVoiceGuild(ulong GuildId, string GuildName, IEnumerable<AutoVoiceChannel> VoiceChannels);

        private readonly IAutoVoiceChannelRepository _repository;
        private readonly DiscordClient _client;

        public AutoVoiceController(IAutoVoiceChannelRepository repository, DiscordClient client)
        {
            _repository = repository;
            _client = client;
        }

        [HttpGet]
        public async Task<IEnumerable<AutoVoiceGuild>> GetVoiceChannels()
        {
            var guilds = new List<AutoVoiceGuild>();

            var autoVoiceGuilds = await _repository.GetAllAsync(_client);

            foreach (var autoVoiceChannels in autoVoiceGuilds)
            {
                var guild = autoVoiceChannels.Key;
                var voiceChannels = autoVoiceChannels
                    .Select(x => new AutoVoiceChannel(x.Id, x.Name, x.Users.Count()))
                    .ToList();
                
                guilds.Add(new AutoVoiceGuild(guild.Id, guild.Name, voiceChannels));
            }

            return guilds;
        }
    }
}