using System;
using System.Collections.Concurrent;
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
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        // Key = voice channel ID
        // Value = should the channel be updated after it's removed from this dictionary
        private readonly ConcurrentDictionary<ulong, bool> _queuedVoiceChannels;

        public AutoVoiceService(DiscordClient client, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _client = client;
            _unitOfWorkFactory = unitOfWorkFactory;

            _queuedVoiceChannels = new ConcurrentDictionary<ulong, bool>();
        }

        public async ValueTask<DiscordChannel> EnableForGuildAsync(DiscordGuild guild, DiscordChannel? category = null)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var voiceChannels = await unitOfWork.AutoVoiceChannels.GetChannelsAsync(guild);

            if (voiceChannels.Count > 0)
                throw new InvalidOperationException("Auto Voice is already enabled on this server.");

            var guildSettings = await unitOfWork.GuildSettings.GetAsync(guild);
            var bitrate = guildSettings?.VoiceBitrate;

            var newVoiceChannel = await CreateNewChannelAsync(guild, category, bitrate);
            
            await unitOfWork.AutoVoiceChannels.AddAsync(newVoiceChannel);
            await unitOfWork.CompleteAsync();

            return newVoiceChannel;
        }

        private async Task<DiscordChannel> CreateNewChannelAsync(DiscordGuild guild, DiscordChannel? category, int? bitrate)
        {
            var overwrites = new List<DiscordOverwriteBuilder>
            {
                new DiscordOverwriteBuilder().For(guild.EveryoneRole).Deny(Permissions.ManageChannels),
                new DiscordOverwriteBuilder().For(guild.CurrentMember).Allow(Permissions.ManageChannels)
            };

            if (bitrate != null)
            {
                bitrate *= 1000;
            }
            
            return await guild.CreateVoiceChannelAsync(NameNew, category, bitrate, overwrites: overwrites);
        }

        public async ValueTask<int> DisableForGuildAsync(DiscordGuild guild)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var voiceChannels = await unitOfWork.AutoVoiceChannels.GetChannelsAsync(guild);

            var count = voiceChannels.Count;
            if (count == 0)
                throw new InvalidOperationException("Auto Voice is already disabled on this server.");
            
            foreach (var voiceChannel in voiceChannels)
            {
                unitOfWork.AutoVoiceChannels.Remove(voiceChannel);
                await voiceChannel.DeleteAsync();
            }

            await unitOfWork.CompleteAsync();

            return count;
        }

        public async ValueTask<IEnumerable<DiscordChannel>> GetByGuildAsync(DiscordGuild guild)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            return await unitOfWork.AutoVoiceChannels.GetChannelsAsync(guild);
        }

        public async ValueTask<DiscordChannel> CreateAsync(DiscordChannel template)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();

            var guildSettings = await unitOfWork.GuildSettings.GetAsync(template.Guild);
            var bitrate = guildSettings?.VoiceBitrate;
            
            var newVoiceChannel = await CreateNewChannelAsync(template.Guild, template.Parent, bitrate);

            await unitOfWork.AutoVoiceChannels.AddAsync(newVoiceChannel);
            await unitOfWork.CompleteAsync();

            return newVoiceChannel;
        }

        public async Task DeleteAsync(DiscordChannel voiceChannel)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            
            unitOfWork.AutoVoiceChannels.Remove(voiceChannel);
            await voiceChannel.DeleteAsync();

            await unitOfWork.CompleteAsync();
        }

        public async Task UpdateNameAsync(DiscordChannel voiceChannel)
        {
            var name = GetBestName(voiceChannel);
            
            if (voiceChannel.Name == name)
            {
                return;
            }

            // Check if the voice channel was recently renamed, and if it was, mark it as "to be updated".
            if (_queuedVoiceChannels.ContainsKey(voiceChannel.Id))
            {
                _queuedVoiceChannels[voiceChannel.Id] = true;
                return;
            }
            
            await voiceChannel.ModifyAsync(x => x.Name = name);

            _queuedVoiceChannels.TryAdd(voiceChannel.Id, false);
            
            _ = DelayRenamingAsync(voiceChannel.Id);
        }

        private async Task DelayRenamingAsync(ulong voiceChannelId)
        {
            await Task.Delay(TimeSpan.FromMinutes(5));

            _queuedVoiceChannels.TryRemove(voiceChannelId, out var shouldUpdate);

            if (shouldUpdate)
            {
                var voiceChannel = await _client.GetChannelAsync(voiceChannelId);
                await UpdateNameAsync(voiceChannel);
            }
        }

        public string GetBestName(DiscordChannel voiceChannel)
        {
            // TODO: Use strategy pattern(?)
            // I mean like having a list of IAutoVoiceNamingStrategy and iterate through them
            // until at least one returns non-null string
            // The interface can have an Order/Priority property because there can be multiple strategies
            // with valid result, example:
            // StreamerNamingStrategy - Returns valid name only when there's at least one person streaming
            // PopularGameNamingStrategy - Return valid name when there's at least one gaming activity among all users
            
            
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

        public async ValueTask<DiscordChannel?> FindAsync(DiscordUser user)
        {
            if (user.IsBot)
                return null;
            
            // TODO: Potential use of cache
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var autoVoiceGuilds = await unitOfWork.AutoVoiceChannels.GetAllAsync(_client);

            foreach (var autoVoiceChannels in autoVoiceGuilds)
            {
                var guild = autoVoiceChannels.Key;
                var member = await guild.GetMemberAsync(user.Id);
                var memberVoiceChannel = member.VoiceState?.Channel;
                
                if (memberVoiceChannel == null)
                    continue;

                if (autoVoiceChannels.Contains(memberVoiceChannel))
                    return memberVoiceChannel;
            }

            return null;
        }

        public async ValueTask<bool> ContainsAsync(DiscordChannel? voiceChannel)
        {
            // TODO: Potential use of cache
            if (voiceChannel == null)
                return false;
            
            await using var unitOfWork = _unitOfWorkFactory.Create();
            return await unitOfWork.AutoVoiceChannels.Contains(voiceChannel);
        }

        public async ValueTask<bool> IsCreatorAsync(DiscordChannel voiceChannel)
        {
            await using var unitOfWork = _unitOfWorkFactory.Create();
            return await unitOfWork.AutoVoiceChannels.GetCreator(voiceChannel.Guild) == voiceChannel;
        }

        public async Task CatchUpAsync()
        {
            // Warning: This method is ugly, when I have more time I will refactor this but now I won't bother.
            
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var autoVoiceGuilds = await unitOfWork.AutoVoiceChannels.GetAllRawAsync();

            foreach (var autoVoiceChannels in autoVoiceGuilds)
            {
                var guild = _client.Guilds[autoVoiceChannels.Key];
                var voiceChannelCreator = autoVoiceChannels.Last();

                // If the voice channel creator was deleted, then disable auto voice for the guild (by deleting all channels).
                var discordVoiceChannelCreator = guild.GetChannel(voiceChannelCreator.VoiceChannelId);
                if (discordVoiceChannelCreator == null)
                {
                    foreach (var autoVoiceChannel in autoVoiceChannels)
                    {
                        var discordVoiceChannel = guild.GetChannel(autoVoiceChannel.VoiceChannelId);

                        unitOfWork.AutoVoiceChannels.Remove(autoVoiceChannel);
                        if (discordVoiceChannel != null)
                        {
                            await discordVoiceChannel.DeleteAsync();
                        }
                    }

                    continue;
                }

                if (discordVoiceChannelCreator.Users.Any())
                {
                    await UpdateNameAsync(discordVoiceChannelCreator);
                    await CreateAsync(discordVoiceChannelCreator);
                }

                // Update all channels
                foreach (var autoVoiceChannel in autoVoiceChannels)
                {
                    if (autoVoiceChannel == voiceChannelCreator)
                        continue;
                    
                    var discordVoiceChannel = guild.GetChannel(autoVoiceChannel.VoiceChannelId);

                    if (discordVoiceChannel == null || !discordVoiceChannel.Users.Any())
                    {
                        unitOfWork.AutoVoiceChannels.Remove(autoVoiceChannel);
                        if (discordVoiceChannel != null)
                        {
                            await discordVoiceChannel.DeleteAsync();
                        }
                    }
                    else
                    {
                        await UpdateNameAsync(discordVoiceChannel);
                    }
                }
            }

            await unitOfWork.CompleteAsync();
        }
    }
}