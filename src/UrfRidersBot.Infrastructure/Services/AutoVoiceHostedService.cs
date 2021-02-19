using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Persistence;
using UrfRidersBot.Persistence.DTOs;

namespace UrfRidersBot.Infrastructure
{
    internal partial class AutoVoiceHostedService : IAutoVoiceHostedService
    {
        private readonly DiscordClient _client;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger<AutoVoiceHostedService> _logger;

        public AutoVoiceHostedService(
            DiscordClient client,
            IUnitOfWorkFactory unitOfWorkFactory,
            ILogger<AutoVoiceHostedService> logger)
        {
            _client = client;
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public async IAsyncEnumerable<DiscordChannel> GetChannels(DiscordGuild guild)
        {
            if (!_guilds.ContainsKey(guild.Id))
                throw new InvalidOperationException("Auto Voice is not enabled on this server.");

            foreach (var channelId in _guilds[guild.Id].ToList())
            {
                yield return await _client.GetChannelAsync(channelId);
            }
        }

        public async ValueTask<DiscordChannel> Enable(DiscordGuild guild, DiscordChannel? category = null)
        {
            if (_guilds.ContainsKey(guild.Id))
                throw new InvalidOperationException("Auto Voice is already enabled on this server.");
            
            return await CreateVoiceChannel(guild, category);
        }

        public async ValueTask<int> Disable(DiscordGuild guild)
        {
            if (!_guilds.ContainsKey(guild.Id))
                throw new InvalidOperationException("Auto Voice is already disabled on this server.");

            // Copy the list first and remove it from memory
            var voiceChannels = _guilds[guild.Id].ToList();
            var count = voiceChannels.Count;
            _guilds.Remove(guild.Id);

            await using var unitOfWork = _unitOfWorkFactory.Create();
            
            foreach (var channelId in voiceChannels)
            {
                var channel = await _client.GetChannelAsync(channelId);
                unitOfWork.AutoVoiceChannels.Remove(channel);
                await channel.DeleteAsync();
            }

            await unitOfWork.CompleteAsync();
            
            return count;
        }

        /// <summary>
        /// Creates new AutoVoice™ channel and stores the information to in-memory dictionary and database.
        /// </summary>
        private async ValueTask<DiscordChannel> CreateVoiceChannel(DiscordGuild guild, DiscordChannel? parent = null)
        {
            // Create in-memory collection if it does not already exist
            if (!_guilds.ContainsKey(guild.Id))
                _guilds[guild.Id] = new List<ulong>();
            
            // Create voice channel on discord
            var voiceChannel = await guild.CreateVoiceChannelAsync(NameNew, parent);

            // Save data
            _guilds[guild.Id].Add(voiceChannel.Id);

            await using var unitOfWork = _unitOfWorkFactory.Create();
            await unitOfWork.AutoVoiceChannels.AddAsync(voiceChannel);
            await unitOfWork.CompleteAsync();

            return voiceChannel;
        }

        /// <summary>
        /// Deletes an AutoVoice™ channel from discord, memory and database.
        /// </summary>
        private async Task DeleteVoiceChannel(DiscordGuild guild, ulong voiceChannelId)
        {
            if (_guilds[guild.Id].Count <= 1 || _guilds[guild.Id].Last() == voiceChannelId)
                return;

            var voiceChannel = guild.GetChannel(voiceChannelId);
            if (voiceChannel != null)
            {
                await using var unitOfWork = _unitOfWorkFactory.Create();
                unitOfWork.AutoVoiceChannels.Remove(voiceChannel);
                await unitOfWork.CompleteAsync();
                await voiceChannel.DeleteAsync();
            }
        }
    }
}