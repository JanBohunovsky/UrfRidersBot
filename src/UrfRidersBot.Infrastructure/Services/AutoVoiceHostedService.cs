using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
{
    internal class AutoVoiceHostedService : IHostedService
    {
        // Probably make these configurable
        private const string NameNew = "➕ New Voice Channel";
        private const string NameGeneral = "🎧 General";
        private const string NameGaming = "🎮 Gaming";
        private const string NameStreaming = "📺 Streaming";
        private const string FormatGaming = "🎮 {0}";
        private const string FormatStreaming = "📺 {0}'s Stream";
        
        // And these as well
        private readonly HashSet<string> _ignoredGames = new()
        {
            "visual studio",
            "visual studio code",
            "intellij idea",
            "intellij idea ultimate",
            "rider",
        };
        
        private readonly DiscordClient _client;
        private readonly IAutoVoiceService _autoVoiceService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        // private Dictionary<ulong, List<ulong>> _guilds = new();

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
            
            // Catch up - remove empty channels (except the last one) and create a new one if needed
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var allSettings = await unitOfWork.AutoVoiceSettings.GetEnabledAsync();

            foreach (var settings in allSettings)
            {
                var guild = e.Guilds[settings.GuildId];
                
                // Check if creator channel has been deleted (if yes, remove all other channels and disable module)
                var voiceCreator = guild.GetChannel(settings.VoiceChannelCreatorId!.Value);
                if (voiceCreator == null)
                {
                    foreach (var autoVoiceChannel in settings.VoiceChannels)
                    {
                        var voiceChannel = guild.GetChannel(autoVoiceChannel.VoiceChannelId);
                        if (voiceChannel != null)
                            await voiceChannel.DeleteAsync();
                    }
                    
                    settings.Disable();
                    continue;
                }
                
                if (voiceCreator.Users.Any())
                {
                    var newVoiceCreator = await guild.CreateVoiceChannelAsync(NameNew, voiceCreator.Parent);
                    settings.AddChannel(newVoiceCreator);
                }
                
                // Remove empty voice channels
                foreach (var autoVoiceChannel in settings.VoiceChannels)
                {
                    var voiceChannel = guild.GetChannel(autoVoiceChannel.VoiceChannelId);

                    if (voiceChannel == null || !voiceChannel.Users.Any())
                    {
                        settings.RemoveChannel(autoVoiceChannel.VoiceChannelId);
                    }
                    else
                    {
                        UpdateChannelName(voiceChannel);
                    }
                }
            }
        }

        /// <summary>
        /// Remove a voice channel from memory and database if it was deleted manually.
        /// </summary>
        private async Task OnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs e)
        {
            if (!_guilds.ContainsKey(e.Guild.Id))
                return;
            
            if (_guilds[e.Guild.Id].Remove(e.Channel.Id))
            {
                await using var unitOfWork = _unitOfWorkFactory.Create();
                unitOfWork.AutoVoiceChannels.Remove(e.Channel);
                await unitOfWork.CompleteAsync();
                
                // If all voice channels have been deleted then disable the module
                if (_guilds[e.Guild.Id].Count == 0)
                    _guilds.Remove(e.Guild.Id);
            }
        }

        private async Task OnPresenceUpdated(DiscordClient sender, PresenceUpdateEventArgs e)
        {
            var voiceChannel = await FindVoiceChannelAsync(e.User);
            if (voiceChannel == null)
                return;

            if (e.PresenceBefore.Activities.Count == e.PresenceAfter.Activities.Count)
                return;

            UpdateChannelName(voiceChannel);
        }

        private async Task OnVoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            // User did not change voice channel
            if (e.Before?.Channel == e.After?.Channel)
                return;

            // Guild does not have auto voice enabled
            if (!_guilds.ContainsKey(e.Guild.Id))
                return;

            // User has joined one of my voice channels
            if (e.After?.Channel != null && _guilds[e.Guild.Id].Contains(e.After.Channel.Id))
            {
                await OnUserJoin(e.After.Channel);
            }
            
            // User has left one of my voice channels
            if (e.Before?.Channel != null && _guilds[e.Guild.Id].Contains(e.Before.Channel.Id))
            {
                await OnUserLeft(e.Before.Channel);
            }
        }

        private async Task OnUserJoin(DiscordChannel voiceChannel)
        {
            // User has joined the last voice channel -> Create new one
            if (_guilds[voiceChannel.GuildId].Last() == voiceChannel.Id)
            {
                await CreateVoiceChannel(voiceChannel.Guild, voiceChannel.Parent);
            }
            
            UpdateChannelName(voiceChannel);
        }

        private async Task OnUserLeft(DiscordChannel voiceChannel)
        {
            // All users left the channel -> Delete it
            if (!voiceChannel.Users.Any())
            {
                await DeleteVoiceChannel(voiceChannel.Guild, voiceChannel.Id);
            }
            else
            {
                UpdateChannelName(voiceChannel);
            }
        }

        private void UpdateChannelName(DiscordChannel voiceChannel)
        {
            // TODO: Add delay:
            // 2 methods: UpdateChannelName & DelayUpdateChannelName (temp name)
            // UpdateChannelName will stay the same as it currently is.
            // DelayUpdateChannelName will require a List (list or dictionary) of voice channels and update times:
            //  1. Check if the channel is in the List
            //    True - do nothing
            //    False - create a new DateTimeOffset by getting the current time and adding a Delay to it and add those values to the List
            //          - Delay: 5 minutes is the safest, this should never hit the rate limit but it will be less responsive
            //                   3 minutes look like the best number, it should be responsive enough while not hitting many rate limits
            
            var name = GetChannelName(voiceChannel);
            
            if (voiceChannel.Name != name)
            {
                // TODO: Figure out what to do with this, the rate limit is 2 requests per 10 minutes....
                _ = voiceChannel.ModifyAsync(x => x.Name = name);
            }
        }

        private string GetChannelName(DiscordChannel voiceChannel)
        {
            var presence = voiceChannel.Users
                .Where(u => !u.IsBot)
                .Select(u => u.Presence)
                .ToList();

            // Get all users that are streaming then:
            // If there's only a single user -> [User]'s Stream
            // If there are multiple users -> Streaming
            var streaming = presence
                .Where(p => p.Activities.Any(a => a.ActivityType == ActivityType.Streaming))
                .ToList();

            if (streaming.Count == 1)
            {
                return string.Format(FormatStreaming, streaming.First().User.Username);
            }
            if (streaming.Count > 1)
            {
                return NameStreaming;
            }
            
            // Get two most popular games (activities of type Playing) then:
            // If there's only a single game OR only a single popular game -> [Game]
            // If there are multiple games that have the same number of players -> Gaming
            // e.g. Two users are playing "League of Legends" and another two are playing "Minecraft",
            // so the channel name would be called "Gaming", but if three people are playing "League of Legends",
            // then the name would be "League of Legends".
            var playing = presence
                .SelectMany(p => p.Activities.Where(a =>
                    a.ActivityType == ActivityType.Playing &&
                    !_ignoredGames.Contains(a.Name.ToLower())))
                .GroupBy(a => a.Name, a => a.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToList();

            // Only a single game -> just show it
            if (playing.Count == 1)
            {
                return string.Format(FormatGaming, playing.First().Name);
            }
            // More than one game..
            if (playing.Count > 1)
            {
                // Figure out the highest number of users playing the same game
                var max = playing.Max(x => x.Count);
                // Get all the games with that number
                var popular = playing
                    .Where(x => x.Count == max)
                    .Select(x => x.Name)
                    .ToList();

                // Only a single game have that number -> show it
                if (popular.Count == 1)
                {
                    return string.Format(FormatGaming, popular.First());
                }
                // Multiple games are played equally -> show "Gaming"
                if (popular.Count > 1)
                {
                    return NameGaming;
                }
            }

            return NameGeneral;
        }

        private async Task<DiscordChannel?> FindVoiceChannelAsync(DiscordUser user)
        {
            // Ignore bots
            if (user.IsBot)
                return null;
            
            foreach (var (guildId, guild) in _client.Guilds)
            {
                if (!_guilds.ContainsKey(guildId))
                    continue;
                
                var member = await guild.GetMemberAsync(user.Id);
                if (member.VoiceState?.Channel == null)
                    continue;

                if (_guilds[guildId].Contains(member.VoiceState.Channel.Id))
                    return member.VoiceState.Channel;
            }
            
            return null;
        }
    }
}