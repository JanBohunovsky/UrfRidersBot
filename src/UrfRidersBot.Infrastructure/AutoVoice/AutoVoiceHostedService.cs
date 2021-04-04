using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Infrastructure.AutoVoice
{
    internal class AutoVoiceHostedService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly IAutoVoiceService _autoVoiceService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public AutoVoiceHostedService(
            DiscordClient client, 
            IAutoVoiceService autoVoiceService,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _client = client;
            _autoVoiceService = autoVoiceService;
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.GuildDownloadCompleted += OnGuildDownloadCompleted;
            _client.PresenceUpdated += OnPresenceUpdated;
            _client.VoiceStateUpdated += OnVoiceStateUpdated;
            _client.ChannelDeleted += OnChannelDeleted;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.PresenceUpdated -= OnPresenceUpdated;
            _client.VoiceStateUpdated -= OnVoiceStateUpdated;
            _client.ChannelDeleted -= OnChannelDeleted;
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Load voice channels from the database on start up.
        /// </summary>
        private async Task OnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
        {
            _client.GuildDownloadCompleted -= OnGuildDownloadCompleted;

            await CatchUpAsync();
        }

        /// <summary>
        /// Remove a voice channel from memory and database if it was deleted manually.
        /// </summary>
        private async Task OnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs e)
        {
            // TODO: Potential use of cache
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var settings = await unitOfWork.AutoVoiceSettings.GetAsync(e.Guild);

            if (settings?.ChannelCreatorId is null)
            {
                return;
            }

            if (settings.RemoveChannel(e.Channel))
            {
                await unitOfWork.CompleteAsync();
            }
        }

        private async Task OnPresenceUpdated(DiscordClient sender, PresenceUpdateEventArgs e)
        {
            var voiceChannel = await FindVoiceChannelAsync(e.User);
            if (voiceChannel is null)
                return;

            if (e.PresenceBefore.Activities.Count == e.PresenceAfter.Activities.Count)
                return;

            await _autoVoiceService.UpdateNameAsync(voiceChannel);
        }

        private async Task OnVoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            // User did not change voice channel
            if (e.Before?.Channel == e.After?.Channel)
                return;

            await using var unitOfWork = _unitOfWorkFactory.Create();
            var settings = await unitOfWork.AutoVoiceSettings.GetAsync(e.Guild);

            if (settings?.ChannelCreatorId is null)
            {
                return;
            }

            await OnUserJoin(e.After?.Channel, settings);
            await OnUserLeft(e.Before?.Channel, settings);

            await unitOfWork.CompleteAsync();
        }

        private async Task OnUserJoin(DiscordChannel? voiceChannel, AutoVoiceSettings settings)
        {
            if (voiceChannel is null)
            {
                return;
            }
            
            if (settings.ContainsChannel(voiceChannel))
            {
                await _autoVoiceService.UpdateNameAsync(voiceChannel);
            }
            else if (settings.ChannelCreatorId == voiceChannel.Id)
            {
                await _autoVoiceService.UpdateNameAsync(voiceChannel);
                
                var newVoiceChannel = await _autoVoiceService.CreateAsync(voiceChannel.Guild, voiceChannel.Parent, settings.Bitrate);
                settings.AddChannel(newVoiceChannel);
            }
        }

        private async Task OnUserLeft(DiscordChannel? voiceChannel, AutoVoiceSettings settings)
        {
            if (voiceChannel is null)
            {
                return;
            }

            if (!settings.ContainsChannel(voiceChannel))
            {
                return;
            }
            
            if (voiceChannel.Users.Any())
            {
                await _autoVoiceService.UpdateNameAsync(voiceChannel);
                return;
            }
            
            // All users left, now we can delete the channel
            if (settings.RemoveChannel(voiceChannel))
            {
                await voiceChannel.DeleteAsync();
            }
        }
        
        private async ValueTask<DiscordChannel?> FindVoiceChannelAsync(DiscordUser user)
        {
            if (user.IsBot)
                return null;
            
            // TODO: Potential use of cache
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var guilds = await unitOfWork.AutoVoiceSettings.GetEnabledAsync();

            foreach (var settings in guilds)
            {
                var guild = _client.Guilds[settings.GuildId];
                var member = await guild.GetMemberAsync(user.Id);
                var memberVoiceChannel = member.VoiceState?.Channel;
                
                // User is not connected to any voice channel
                if (memberVoiceChannel is null)
                    continue;

                // User is connected to an auto voice channel
                if (settings.ContainsChannel(memberVoiceChannel))
                    return memberVoiceChannel;
            }

            return null;
        }
        
        private async Task CatchUpAsync()
        {
            // Warning: This method is ugly, when I have more time I will refactor this but now I won't bother.
            
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var guilds = await unitOfWork.AutoVoiceSettings.GetEnabledAsync();

            foreach (var settings in guilds)
            {
                var guild = _client.Guilds[settings.GuildId];
                var channelCreator = settings.GetChannelCreator(guild);

                // If the voice channel creator was deleted, then disable auto voice for the guild (by deleting all channels).
                if (channelCreator is null)
                {
                    await DisableModuleAsync(guild, settings);
                    continue;
                }

                // Channel creator is not empty, that means we have to create new channel.
                if (channelCreator.Users.Any())
                {
                    await _autoVoiceService.UpdateNameAsync(channelCreator);
                    var newVoiceChannel = await _autoVoiceService.CreateAsync(guild, channelCreator.Parent, settings.Bitrate);
                    settings.AddChannel(newVoiceChannel);
                }

                // Update all channels
                await UpdateAutoVoiceChannels(guild, settings);
            }

            await unitOfWork.CompleteAsync();
        }

        private async Task DisableModuleAsync(DiscordGuild guild, AutoVoiceSettings settings)
        {
            foreach (var voiceChannelId in settings.VoiceChannels.Select(v => v.VoiceChannelId))
            {
                var voiceChannel = guild.GetChannel(voiceChannelId);
                if (voiceChannel is not null)
                {
                    await voiceChannel.DeleteAsync();
                }
            }

            settings.ChannelCreatorId = null;
            settings.RemoveAllChannels();
        }

        private async Task UpdateAutoVoiceChannels(DiscordGuild guild, AutoVoiceSettings settings)
        {
            var voiceChannelIds = settings.VoiceChannels
                .Select(v => v.VoiceChannelId)
                .ToList();
                
            foreach (var voiceChannelId in voiceChannelIds)
            {
                var voiceChannel = guild.GetChannel(voiceChannelId);

                if (voiceChannel is null)
                {
                    settings.RemoveChannel(voiceChannelId);
                } 
                else if (!voiceChannel.Users.Any())
                {
                    settings.RemoveChannel(voiceChannelId);
                    await voiceChannel.DeleteAsync();
                }
                else
                {
                    await _autoVoiceService.UpdateNameAsync(voiceChannel);
                }
            }
        }
    }
}