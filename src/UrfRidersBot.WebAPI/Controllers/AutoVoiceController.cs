using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.AspNetCore.Mvc;
using UrfRidersBot.Infrastructure;

namespace UrfRidersBot.WebAPI.Controllers
{
    // Temporary: Just for testing the API
    [Route("api/[controller]")]
    [ApiController]
    public class AutoVoiceController : Controller
    {
        public record AutoVoiceChannel(ulong ChannelId, string ChannelName, int UserCount);

        public record AutoVoiceGuild(ulong GuildId, string GuildName, IEnumerable<AutoVoiceChannel> VoiceChannels);

        private readonly DiscordClient _client;
        private readonly IAutoVoiceService _autoVoiceService;

        public AutoVoiceController(IAutoVoiceService autoVoiceService, DiscordClient client)
        {
            _autoVoiceService = autoVoiceService;
            _client = client;
        }

        [HttpGet]
        public async Task<IEnumerable<AutoVoiceGuild>> GetVoiceChannels()
        {
            var guilds = new List<AutoVoiceGuild>();
            
            foreach (var (id, guild) in _client.Guilds)
            {
                var voiceChannels = new List<AutoVoiceChannel>();

                try
                {
                    await foreach (var channel in _autoVoiceService.GetChannels(guild))
                    {
                        voiceChannels.Add(new AutoVoiceChannel(channel.Id, channel.Name, channel.Users.Count()));
                    }
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
                
                guilds.Add(new AutoVoiceGuild(guild.Id, guild.Name, voiceChannels));
            }

            return guilds;
        }
    }
}