using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
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

        [HttpGet("{guildId}/{channelId}/{messageId}")]
        public async Task<IActionResult> Get(ulong guildId, ulong channelId, ulong messageId)
        {
            if (!_client.Guilds.TryGetValue(guildId, out var guild))
                return NotFound("Guild not found.");

            if (!guild.Channels.TryGetValue(channelId, out var channel))
                return NotFound("Channel not found.");

            DiscordMessage? message;
            try
            {
                message = await channel.GetMessageAsync(messageId);
            }
            catch (UnauthorizedException e)
            {
                return NotFound("Message not found.");
            }

            return Ok(MessageDTO.FromDiscord(message));
        }
        
        [HttpPost("{guildId}/{channelId}")]
        public async Task<IActionResult> Send(ulong guildId, ulong channelId, [FromBody] MessageDTO model)
        {
            if (!_client.Guilds.TryGetValue(guildId, out var guild))
                return NotFound("Guild not found.");

            if (!guild.Channels.TryGetValue(channelId, out var channel))
                return NotFound("Channel not found.");

            DiscordMessage? message;
            try
            {
                message = await channel.SendMessageAsync(model.Content, model.Embed?.ToDiscord());
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            return Created($"/api/messages/{message.ChannelId}/{message.Id}", MessageDTO.FromDiscord(message));
        }

        [HttpPut("{guildId}/{channelId}/{messageId}")]
        public async Task<IActionResult> Modify(ulong guildId, ulong channelId, ulong messageId, [FromBody] MessageDTO model)
        {
            if (!_client.Guilds.TryGetValue(guildId, out var guild))
                return NotFound("Guild not found.");

            if (!guild.Channels.TryGetValue(channelId, out var channel))
                return NotFound("Channel not found.");

            var message = await channel.GetMessageAsync(messageId);
            if (message == null)
                return NotFound("Message not found.");

            try
            {
                await message.ModifyAsync(model.Content, model.Embed?.ToDiscord().Build());
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (UnauthorizedException e)
            {
                return BadRequest(e.JsonMessage);
            }

            return Ok();
        }
    }
}