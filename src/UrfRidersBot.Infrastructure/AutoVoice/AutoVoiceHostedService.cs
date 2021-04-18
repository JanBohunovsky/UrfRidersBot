using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.Common.Configuration;

namespace UrfRidersBot.Infrastructure.AutoVoice
{
    internal class AutoVoiceHostedService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly IOptionsMonitor<DiscordOptions> _options;
        private readonly IAutoVoiceService _autoVoiceService;
        private readonly IRepositoryFactory<IAutoVoiceSettingsRepository> _factory;

        public AutoVoiceHostedService(
            DiscordClient client,
            IOptionsMonitor<DiscordOptions> options,
            IAutoVoiceService autoVoiceService,
            IRepositoryFactory<IAutoVoiceSettingsRepository> factory)
        {
            _client = client;
            _options = options;
            _autoVoiceService = autoVoiceService;
            _factory = factory;
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
            using var repository = _factory.Create();
            var settings = await repository.GetAsync();

            if (settings?.ChannelCreator is null)
            {
                return;
            }

            if (settings.RemoveChannel(e.Channel))
            {
                await repository.SaveAsync(settings);
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

            // Only do work in configured guild
            if (_options.CurrentValue.GuildId != e.Guild.Id)
                return;

            using var repository = _factory.Create();
            var settings = await repository.GetAsync();

            if (settings?.ChannelCreator is null)
            {
                return;
            }

            await OnUserJoin(e.After?.Channel, settings, repository);
            await OnUserLeft(e.Before?.Channel, settings, repository);
        }

        private async Task OnUserJoin(DiscordChannel? voiceChannel, AutoVoiceSettings settings, IAutoVoiceSettingsRepository repository)
        {
            if (voiceChannel is null)
            {
                return;
            }
            
            if (settings.ContainsChannel(voiceChannel))
            {
                await _autoVoiceService.UpdateNameAsync(voiceChannel);
            }
            else if (settings.ChannelCreator == voiceChannel)
            {
                await _autoVoiceService.UpdateNameAsync(voiceChannel);
                
                var newVoiceChannel = await _autoVoiceService.CreateAsync(voiceChannel.Guild, voiceChannel.Parent, settings.Bitrate);
                
                settings.AddChannel(newVoiceChannel);
                await repository.SaveAsync(settings);
            }
        }

        private async Task OnUserLeft(DiscordChannel? voiceChannel, AutoVoiceSettings settings, IAutoVoiceSettingsRepository repository)
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
                await repository.SaveAsync(settings);
            }
        }
        
        private async ValueTask<DiscordChannel?> FindVoiceChannelAsync(DiscordUser user)
        {
            if (user.IsBot)
                return null;
            
            using var repository = _factory.Create();
            var settings = await repository.GetAsync();

            // AutoVoice is disabled
            if (settings?.ChannelCreator is null)
            {
                return null;
            }

            var guild = _client.Guilds[_options.CurrentValue.GuildId];
            var member = await guild.GetMemberAsync(user.Id);
            var memberVoiceChannel = member?.VoiceState?.Channel;
                
            // User is not connected to any voice channel
            if (memberVoiceChannel is null)
                return null;

            // User is connected to an auto voice channel
            if (settings.ContainsChannel(memberVoiceChannel))
                return memberVoiceChannel;

            return null;
        }
        
        private async Task CatchUpAsync()
        {
            using var repository = _factory.Create();
            
            await repository.CleanupAsync();
            var settings = await repository.GetAsync();

            if (settings?.ChannelCreator is null)
            {
                return;
            }

            var guild = _client.Guilds[_options.CurrentValue.GuildId];
            
            // Channel creator is not empty, that means we have to create new channel.
            if (settings.ChannelCreator.Users.Any())
            {
                await _autoVoiceService.UpdateNameAsync(settings.ChannelCreator);
                var newVoiceChannel = await _autoVoiceService.CreateAsync(guild, settings.ChannelCreator.Parent, settings.Bitrate);
                
                settings.AddChannel(newVoiceChannel);
            }

            // Update all channels
            await UpdateAutoVoiceChannels(settings);
            
            await repository.SaveAsync(settings);
        }

        private async Task UpdateAutoVoiceChannels(AutoVoiceSettings settings)
        { 
            foreach (var voiceChannel in settings.VoiceChannels)
            {
                if (!voiceChannel.Users.Any())
                {
                    settings.RemoveChannel(voiceChannel);
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