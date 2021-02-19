using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
{
    public class AutoVoiceService : IAutoVoiceService
    {
        private readonly DiscordClient _client;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        // private readonly Dictionary<ulong, List<ulong>> _guildCache;

        public AutoVoiceService(DiscordClient client, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _client = client;
            _unitOfWorkFactory = unitOfWorkFactory;
            
            // _guildCache = new Dictionary<ulong, List<ulong>>();
        }

        public ValueTask<DiscordChannel> EnableForGuildAsync(DiscordGuild guild, DiscordChannel? category = null)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<int> DisableForGuildAsync(DiscordGuild guild)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<IEnumerable<DiscordChannel>> GetGuildVoiceChannels(DiscordGuild guild)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask<DiscordChannel> CreateVoiceChannelAsync(DiscordGuild guild, DiscordChannel? category = null)
        {
            var voiceChannel = await guild.CreateVoiceChannelAsync("🟢 New Voice Channel", category);
            
            await using var unitOfWork = _unitOfWorkFactory.Create();
            await unitOfWork.AutoVoiceChannels.AddAsync(voiceChannel);
            await unitOfWork.CompleteAsync();

            return voiceChannel;
        }

        public Task DeleteVoiceChannelAsync(DiscordChannel channel)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateVoiceChannelNameAsync(DiscordChannel channel, string name)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<DiscordChannel> FindVoiceChannelAsync(DiscordUser user)
        {
            throw new System.NotImplementedException();
        }
    }
}