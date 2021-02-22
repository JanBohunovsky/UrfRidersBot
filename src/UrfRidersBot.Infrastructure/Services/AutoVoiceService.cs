using System;
using System.Collections.Generic;
using System.Linq;
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

        public async ValueTask<DiscordChannel> EnableForGuildAsync(DiscordGuild guild, string channelName, DiscordChannel? category = null)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var settings = await unitOfWork.AutoVoiceSettings.GetByGuildAsync(guild);

            if (settings.IsEnabled)
                throw new InvalidOperationException("Auto Voice is already enabled on this server.");

            var voiceCreator = await guild.CreateVoiceChannelAsync(channelName, category);
            settings.Enable(voiceCreator);
            await unitOfWork.CompleteAsync();

            return voiceCreator;
        }

        public async ValueTask<int> DisableForGuildAsync(DiscordGuild guild)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var settings = await unitOfWork.AutoVoiceSettings.GetByGuildAsync(guild);

            if (!settings.IsEnabled)
                throw new InvalidOperationException("Auto Voice is already disabled on this server.");

            await settings.GetVoiceChannelCreator(guild).DeleteAsync();

            foreach (var autoVoiceChannel in settings.VoiceChannels)
            {
                await guild.Channels[autoVoiceChannel.VoiceChannelId].DeleteAsync();
            }

            var count = settings.VoiceChannels.Count + 1;
            
            settings.Disable();
            await unitOfWork.CompleteAsync();

            return count;
        }

        public async ValueTask<IEnumerable<DiscordChannel>> GetGuildVoiceChannels(DiscordGuild guild)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var settings = await unitOfWork.AutoVoiceSettings.GetByGuildAsync(guild);

            if (!settings.IsEnabled)
                throw new InvalidOperationException("Auto Voice is not enabled on this server.");

            return settings.VoiceChannels.Select(x => guild.Channels[x.VoiceChannelId]);
        }

        public async Task CreateVoiceChannelAsync(DiscordGuild guild, string name)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var settings = await unitOfWork.AutoVoiceSettings.GetByGuildAsync(guild);

            if (!settings.IsEnabled)
                return;

            var voiceCreator = settings.GetVoiceChannelCreator(guild);
            var newVoiceCreator = await guild.CreateVoiceChannelAsync(name, voiceCreator.Parent);
            
            settings.AddChannel(newVoiceCreator);
            
            await unitOfWork.CompleteAsync();
        }

        public async Task DeleteVoiceChannelAsync(DiscordChannel channel)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var settings = await unitOfWork.AutoVoiceSettings.GetByGuildAsync(channel.Guild);

            if (!settings.IsEnabled)
                return;
            
            settings.RemoveChannel(channel);
            await channel.DeleteAsync();

            await unitOfWork.CompleteAsync();
        }

        public async Task UpdateVoiceChannelNameAsync(DiscordChannel channel, string name)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<DiscordChannel?> FindVoiceChannelAsync(DiscordClient client, DiscordUser user)
        {
            if (user.IsBot)
                return null;
            
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var allSettings = await unitOfWork.AutoVoiceSettings.GetEnabledAsync();

            foreach (var settings in allSettings)
            {
                var guild = client.Guilds[settings.GuildId];
                var member = await guild.GetMemberAsync(user.Id);
                var voiceChannel = member.VoiceState?.Channel;
                
                if (voiceChannel == null)
                    continue;

                if (guild.Channels.ContainsKey(voiceChannel.Id))
                    return voiceChannel;
            }

            return null;
        }
    }
}