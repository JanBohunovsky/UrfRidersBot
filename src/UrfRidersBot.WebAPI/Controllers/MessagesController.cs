using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Mvc;
using UrfRidersBot.WebAPI.Models;

namespace UrfRidersBot.WebAPI.Controllers
{
    // TODO: Add support for message editing and deleting
    public class MessagesController : ApiController
    {
        private readonly DiscordClient _client;

        public MessagesController(DiscordClient client)
        {
            _client = client;
        }
        
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageDTO message)
        {
            if (!_client.Guilds.TryGetValue(message.GuildId, out var guild))
                return NotFound("Guild not found.");

            if (!guild.Channels.TryGetValue(message.ChannelId, out var channel))
                return NotFound("Channel not found.");

            DiscordMessage? result;
            try
            {
                result = await channel.SendMessageAsync(message.Content, message.Embed?.ToDiscord());
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            return Ok(result.Id);
        }
    }
}