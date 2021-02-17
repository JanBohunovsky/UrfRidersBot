using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Persistence;

namespace UrfRidersBot.Discord
{
    internal partial class AutoVoiceService : IAutoVoiceService
    {
        private readonly DiscordClient _client;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;
        private readonly ILogger<AutoVoiceService> _logger;

        public AutoVoiceService(
            DiscordClient client,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory,
            ILogger<AutoVoiceService> logger)
        {
            _client = client;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async IAsyncEnumerable<DiscordChannel> GetChannels(DiscordGuild guild)
        {
            if (!_voiceChannels.ContainsKey(guild.Id))
                throw new InvalidOperationException("Auto Voice is not enabled on this server.");

            foreach (var channelId in _voiceChannels[guild.Id].ToList())
            {
                yield return await _client.GetChannelAsync(channelId);
            }
        }

        public async ValueTask<DiscordChannel> Enable(DiscordGuild guild, DiscordChannel? category = null)
        {
            if (_voiceChannels.ContainsKey(guild.Id))
                throw new InvalidOperationException("Auto Voice is already enabled on this server.");
            
            return await CreateVoiceChannel(guild, category);
        }

        public async ValueTask<int> Disable(DiscordGuild guild)
        {
            if (!_voiceChannels.ContainsKey(guild.Id))
                throw new InvalidOperationException("Auto Voice is already disabled on this server.");

            // Copy the list first and remove it from memory
            var voiceChannels = _voiceChannels[guild.Id].ToList();
            var count = voiceChannels.Count;
            _voiceChannels.Remove(guild.Id);

            // Delete from discord
            foreach (var channelId in voiceChannels)
            {
                var channel = await _client.GetChannelAsync(channelId);
                await channel.DeleteAsync();
            }
            
            // Remove from database
            await using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.AutoVoiceChannels
                .Where(x => x.GuildId == guild.Id)
                .ForEachAsync(x => dbContext.Remove(x));
            await dbContext.SaveChangesAsync();
            
            return count;
        }

        /// <summary>
        /// Creates new AutoVoice™ channel and stores the information to in-memory dictionary and database.
        /// </summary>
        private async ValueTask<DiscordChannel> CreateVoiceChannel(DiscordGuild guild, DiscordChannel? parent = null)
        {
            // Create in-memory collection if it does not already exist
            if (!_voiceChannels.ContainsKey(guild.Id))
                _voiceChannels[guild.Id] = new List<ulong>();
            
            // Create voice channel on discord
            var voiceChannel = await guild.CreateVoiceChannelAsync(NameNew, parent);

            // Save data
            _voiceChannels[guild.Id].Add(voiceChannel.Id);
            
            await using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.AutoVoiceChannels.AddAsync(new AutoVoiceChannel(guild.Id, voiceChannel.Id));
            await dbContext.SaveChangesAsync();

            return voiceChannel;
        }

        /// <summary>
        /// Deletes an AutoVoice™ channel from discord, memory and database.
        /// </summary>
        private async Task DeleteVoiceChannel(DiscordGuild guild, ulong voiceChannelId)
        {
            if (_voiceChannels[guild.Id].Count <= 1 || _voiceChannels[guild.Id].Last() == voiceChannelId)
                return;
            
            // Remove the voice channel from the memory and database first
            if (_voiceChannels[guild.Id].Remove(voiceChannelId))
            {
                await using var dbContext = _dbContextFactory.CreateDbContext();
                var dbEntity = await dbContext.AutoVoiceChannels.FindAsync(guild.Id, voiceChannelId);
                if (dbEntity != null)
                {
                    dbContext.AutoVoiceChannels.Remove(dbEntity);
                    await dbContext.SaveChangesAsync();
                }
            }
            
            // Then delete it from discord
            var voiceChannel = guild.GetChannel(voiceChannelId);
            if (voiceChannel != null)
            {
                await voiceChannel.DeleteAsync();
            }
        }
    }
}