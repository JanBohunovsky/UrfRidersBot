using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.HostedServices
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

            await _autoVoiceService.CatchUpAsync();
        }

        /// <summary>
        /// Remove a voice channel from memory and database if it was deleted manually.
        /// </summary>
        private async Task OnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs e)
        {
            // TODO: Potential use of cache
            await using var unitOfWork = _unitOfWorkFactory.Create();
            unitOfWork.AutoVoiceChannels.Remove(e.Channel);
            await unitOfWork.CompleteAsync();
        }

        private async Task OnPresenceUpdated(DiscordClient sender, PresenceUpdateEventArgs e)
        {
            var voiceChannel = await _autoVoiceService.FindAsync(e.User);
            if (voiceChannel == null)
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

            // User has joined one of my voice channels
            if (await _autoVoiceService.ContainsAsync(e.After?.Channel))
            {
                await OnUserJoin(e.After!.Channel);
            }
            
            // User has left one of my voice channels
            if (await _autoVoiceService.ContainsAsync(e.Before?.Channel))
            {
                await OnUserLeft(e.Before!.Channel);
            }
        }

        private async Task OnUserJoin(DiscordChannel voiceChannel)
        {
            await _autoVoiceService.UpdateNameAsync(voiceChannel);
            
            // User has joined the last voice channel -> Create new one
            if (await _autoVoiceService.IsCreatorAsync(voiceChannel))
            {
                await _autoVoiceService.CreateAsync(voiceChannel);
            }
        }

        private async Task OnUserLeft(DiscordChannel voiceChannel)
        {
            // All users left the channel -> Delete it
            if (!voiceChannel.Users.Any())
            {
                await _autoVoiceService.DeleteAsync(voiceChannel);
            }
            else
            {
                await _autoVoiceService.UpdateNameAsync(voiceChannel);
            }
        }
    }
}