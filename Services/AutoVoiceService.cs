using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace UrfRiders.Services
{
    public class AutoVoiceService
    {
        public const string NameDefault = "🎧 General";
        public const string NamePlaying = "🎮 {0}";
        public const string NameGaming = "🎮 Gaming";
        public const string NameProgramming = "💻 Programming";
        private readonly string[] _programmingApps = { "visual studio", "intellij" };

        private readonly DiscordSocketClient _client;
        private readonly LiteDatabase _database;
        private readonly ILogger _logger;

        private bool _init;

        public AutoVoiceService(DiscordSocketClient client, LiteDatabase database, ILogger<AutoVoiceService> logger)
        {
            _client = client;
            _database = database;
            _logger = logger;

            _client.UserVoiceStateUpdated += UserVoiceStateUpdated;

            // Run periodic update when the client is ready
            _client.Ready += () =>
            {
                if (!_init)
                {
                    Task.Run(CheckChannels);
                    Task.Run(PeriodicUpdate);
                    _init = true;
                }
                return Task.CompletedTask;
            };
        }

        #region Module Configuration
        /// <summary>
        /// Enables service for specific server.
        /// This function creates new voice channel and updates server settings.
        /// </summary>
        public async Task<bool> Enable(IGuild guild)
        {
            var settings = new ServerSettings(guild.Id, _database);
            if (settings.AutoVoiceChannels.Count > 0)
                return false;

            var voice = await guild.CreateVoiceChannelAsync(NameDefault);
            settings.AutoVoiceChannels.Add(voice.Id);
            settings.Save();
            return true;
        }

        /// <summary>
        /// Disables service for specific server.
        /// This function removes all voice channels created by this service and updates server settings.
        /// </summary>
        public bool Disable(IGuild guild)
        {
            var settings = new ServerSettings(guild.Id, _database);
            if (settings.AutoVoiceChannels.Count == 0)
                return false;

            Task.Run(async () =>
            {
                foreach (var channelId in settings.AutoVoiceChannels)
                {
                    try
                    {
                        var channel = await guild.GetVoiceChannelAsync(channelId);
                        await channel.DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Could not delete voice channel.");
                    }
                }

                settings.AutoVoiceChannels.Clear();
                settings.Save();
            });
            return true;
        }
        #endregion

        #region Channel Naming
        /// <summary>
        /// Periodically update channel names.
        /// </summary>
        private async Task PeriodicUpdate()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    foreach (var settings in ServerSettings.All(_client, _database))
                    {
                        foreach (var channelId in settings.AutoVoiceChannels)
                        {
                            if (_client.GetChannel(channelId) is SocketVoiceChannel channel)
                                await UpdateChannelName(channel);
                        }
                    }
                }
                catch (Exception e)
                {
                    // if periodic check failed, just skip it
                    _logger.LogWarning(e, "Periodic update failed.");
                }
            }
        }

        /// <summary>
        /// Sets channel's name based on user activity.
        /// If there are two or more activities equally frequent then sets generic name.
        /// Programming applications sets name to Programming instead.
        /// </summary>
        private async Task UpdateChannelName(SocketVoiceChannel channel)
        {
            var games = channel.Users
                .Where(u => !u.IsBot && u.Activity != null && (u.Activity.Type == ActivityType.Playing))
                .Select(u => u.Activity.Name)
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Take(2)
                .ToList();

            if (games.Count < 1)
            {
                await RenameChannel(channel, NameDefault);
                return;
            }

            if (games.Count > 1 && games[0].Count() == games[1].Count())
            {
                // Two or more games are played equally, find out if there are any programming apps
                var programming1 = _programmingApps.Any(app => games[0].Key.ToLower().StartsWith(app));
                var programming2 = _programmingApps.Any(app => games[1].Key.ToLower().StartsWith(app));

                if (programming1 || programming2)
                    await RenameChannel(channel, NameDefault); // One of them is programming -> General
                else
                    await RenameChannel(channel, NameGaming); // None of them are programming -> Gaming
            }
            else
            {
                // One popular activity, set that name
                var game = games[0];
                if (_programmingApps.Any(a => game.Key.ToLower().StartsWith(a))) // check programming apps -> set generic name (Programming)
                    await RenameChannel(channel, NameProgramming);
                else
                    await RenameChannel(channel, string.Format(NamePlaying, game.Key));
            }
        }

        /// <summary>
        /// Renames channel to chosen name. Will not rename if channel is already named like that.
        /// </summary>
        private async Task RenameChannel(SocketVoiceChannel channel, string name)
        {
#if DEBUG
            name = $"{name} [DEV]";
#endif
            if (channel.Name == name)
                return;

            await channel.ModifyAsync(v => v.Name = name);
            //_logger.LogDebug($"Renamed {channel.Id} to '{name}'");
        }
        #endregion

        #region Managing Channels
        /// <summary>
        /// This method should be called when bot starts. Cleans up all empty channel while the bot was offline and creates a new one if all are taken.
        /// </summary>
        /// <returns></returns>
        private async Task CheckChannels()
        {
            foreach (var settings in ServerSettings.All(_client, _database))
            {
                var lastChannel = settings.AutoVoiceChannels.Last();
                foreach (var channelId in settings.AutoVoiceChannels)
                {
                    if (!(_client.GetChannel(channelId) is SocketVoiceChannel channel))
                        continue;

                    if (channelId == lastChannel)
                    {
                        if (channel.Users.Count > 0)
                            await UserJoined(channel);
                    }
                    else if (channel.Users.Count == 0)
                    {
                        await UserLeft(channel);
                    }
                }
            }
        }

        private async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (before.VoiceChannel == after.VoiceChannel)
                return;

            if (after.VoiceChannel != null)
            {
                await UserJoined(after.VoiceChannel);
            }
            if (before.VoiceChannel != null)
            {
                await UserLeft(before.VoiceChannel);
            }
        }

        private async Task UserJoined(SocketVoiceChannel voiceChannel)
        {
            var settings = new ServerSettings(voiceChannel.Guild.Id, _database);
            if (settings.AutoVoiceChannels.Count == 0)
                return;

            // Joined last available voice channel -> Create new one
            if (settings.AutoVoiceChannels.Last() == voiceChannel.Id)
            {
                var voiceName = NameDefault;
#if DEBUG
                voiceName = $"{voiceName} [DEV]";
#endif
                var newVoice = await voiceChannel.Guild.CreateVoiceChannelAsync(voiceName, v =>
                {
                    v.Position = voiceChannel.Position;
                    v.CategoryId = voiceChannel.CategoryId;
                });
                settings.AutoVoiceChannels.Add(newVoice.Id);
                settings.Save();
            }

            // Update channel name
            if (settings.AutoVoiceChannels.Contains(voiceChannel.Id))
            {
                await UpdateChannelName(voiceChannel);
            }
        }

        private async Task UserLeft(SocketVoiceChannel voiceChannel)
        {
            var settings = new ServerSettings(voiceChannel.Guild.Id, _database);
            if (settings.AutoVoiceChannels.Count == 0)
                return;

            // User left one of my channels -> Remove or update name
            if (!settings.AutoVoiceChannels.Contains(voiceChannel.Id))
                return;
            // Check if user somehow left last available channel
            if (settings.AutoVoiceChannels.Count == 1)
                return;

            // Last user left -> remove
            if (voiceChannel.Users.Count == 0)
            {
                settings.AutoVoiceChannels.Remove(voiceChannel.Id);
                settings.Save();
                await voiceChannel.DeleteAsync();
            }
            else
            {
                // There are still some user left -> update name
                await UpdateChannelName(voiceChannel);
            }
        }
        #endregion
    }
}