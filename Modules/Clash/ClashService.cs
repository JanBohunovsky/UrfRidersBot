using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.Util;
using UrfRiders.Modules.Settings;

namespace UrfRiders.Modules.Clash
{
    public class ClashService
    {
        public const string TitleUpcomingTournaments = "Upcoming tournaments";
        public const string TitleNewTournaments = "New tournaments available";
        public const string DescriptionNoTournaments = "No planned tournaments.";

        public ClashTournamentData[] Tournaments { get; private set; }

        // When does a day start.
        private readonly TimeSpan _morning = new TimeSpan(6, 0, 0);

        private readonly DiscordSocketClient _client;
        private readonly LiteDatabase _database;
        private readonly ILogger _logger;
        private readonly RiotApi _riotApi;

        private readonly ILiteCollection<ClashTournamentData> _collection;

        private bool _init;

        public ClashService(DiscordSocketClient client, LiteDatabase database, ILogger<ClashService> logger, RiotApi riotApi, CommandService commands)
        {
            _client = client;
            _database = database;
            _logger = logger;
            _riotApi = riotApi;
            _collection = _database.GetCollection<ClashTournamentData>();
            Tournaments = _collection.FindAll().OrderBy(t => t.RegistrationTime).ToArray();

            // Run periodic update when the client is ready
            _client.Ready += async () =>
            {
                if (_init)
                    return;

                // Check Riot API key
                try
                {
                    await _riotApi.LolStatusV3.GetShardDataAsync(Region.EUNE);
                }
                catch (RiotResponseException e)
                {
                    var statusCode = e.GetResponse().StatusCode;
                    if (statusCode == HttpStatusCode.Unauthorized || statusCode == HttpStatusCode.Forbidden)
                    {
                        _logger.LogInformation("Riot API key is invalid or missing, disabling Clash module...");
                        await commands.RemoveModuleAsync<ClashModule>();
                        return;
                    }
                }
                finally
                {
                    _init = true;
                }

                // API key is ok, start periodic updates
                _ = Task.Run(PeriodicDownload);
                _ = Task.Run(PeriodicMorningUpdate);

            };
        }

        public EmbedBuilder CreateTournamentListEmbed(string title, Func<ClashTournamentData, bool> predicate = null)
        {
            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle(title)
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/717788228899307551/717788431006302218/Clash_Crest_icon.webp");

            // Take every tournament by default
            IEnumerable<ClashTournamentData> data = Tournaments;
            // Filter tournaments
            if (predicate != null)
                data = Tournaments.Where(predicate);

            // Add fields
            foreach (var tournament in data)
            {
                embedBuilder.AddField(tournament.FormattedTime, tournament.FormattedName);
            }

            return embedBuilder;
        }

        public async Task UpdateChannelTopic(SocketTextChannel channel)
        {
            if (channel == null)
                return;
            if (Tournaments.Length < 1)
            {
                if (channel.Topic != DescriptionNoTournaments)
                    await channel.ModifyAsync(c => c.Topic = DescriptionNoTournaments);
                return;
            }

            // This array is always sorted by newest first.
            var nextTournament = Tournaments[0];

            var now = DateTimeOffset.Now;
            var start = nextTournament.StartTime;
            var today = new DateTimeOffset(now.Year, now.Month, now.Day, _morning.Hours, _morning.Minutes, _morning.Seconds, now.Offset);
            var target = new DateTimeOffset(start.Year, start.Month, start.Day, _morning.Hours, _morning.Minutes, _morning.Seconds, start.Offset);

            // Calculate remaining days.
            // If current time is less than `_morning` then add 1 day (because we are still in "yesterday").
            var countdown = target - today;
            var days = countdown.TotalDays + (now < today ? 1 : 0);

            var countdownText = target.ToString("M");
            if (days <= 0)
                countdownText = "Today";
            else if (days <= 1)
                countdownText = "Tomorrow";
            else if (days <= 14)
                countdownText = $"In {countdown.Days} days";

            if (channel.Topic == countdownText)
                return;
            await channel.ModifyAsync(c => c.Topic = $"**{countdownText}**: {nextTournament.FormattedName}");
        }

        private async Task<ClashTournamentData[]> DownloadTournaments()
        {
            var data = await _riotApi.ClashV1.GetTournamentsAsync(Region.EUNE);
            return data.Select(ClashTournamentData.Parse).OrderBy(t => t.RegistrationTime).ToArray();
        }

        /// <summary>
        /// Updates channel topic every morning and posts important updates.
        /// </summary>
        private async Task PeriodicMorningUpdate()
        {
            while (true)
            {
                // Wait until the morning
                var now = DateTimeOffset.Now;
                var target = new DateTimeOffset(now.Year, now.Month, now.Day, _morning.Hours, _morning.Minutes, _morning.Seconds, now.Offset);
                target = target.AddSeconds(30);

                var countdown = target - now;
                if (countdown.TotalHours <= 0)
                {
                    target = target.AddDays(1);
                    countdown = target - now;
                }
                _logger.LogDebug($"Next morning update at {target:T}");
                await Task.Delay(countdown);

                // Update channel topic
                foreach (var settings in ServerSettings.All(_client, _database))
                {
                    if (!settings.ClashChannel.HasValue)
                        continue;
                    if (!(_client.GetChannel(settings.ClashChannel.Value) is SocketTextChannel channel))
                        continue;
                    await UpdateChannelTopic(channel);
                }
            }
        }

        /// <summary>
        /// Downloads tournaments every full hour, then notifies servers about new tournaments available.
        /// </summary>
        private async Task PeriodicDownload()
        {
            while (true)
            {
                var now = DateTimeOffset.Now;
                var target = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 30, now.Offset);

                // Calculate target (next full hour)
                target = target.AddHours(1);
                _logger.LogDebug($"Next download at {target:T}");
                await Task.Delay(target - now);

                // Download data
                try
                {
                    Tournaments = await DownloadTournaments();
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Could not fetch upcoming tournaments.");
                    continue;
                }

                // Update database
                _collection.DeleteMany(t => true);
                _collection.InsertBulk(Tournaments);

                // Update servers
                var tournamentIds = Tournaments.Select(t => t.TournamentId).ToList();
                foreach (var settings in ServerSettings.All(_client, _database))
                {
                    if (!settings.ClashChannel.HasValue)
                        continue;
                    if (!(_client.GetChannel(settings.ClashChannel.Value) is SocketTextChannel channel))
                        continue;
                    // Nothing changed => skip
                    if (settings.SeenTournaments.SequenceEqual(tournamentIds))
                        continue;

                    var embed = CreateTournamentListEmbed(TitleNewTournaments, t => !settings.SeenTournaments.Contains(t.TournamentId));

                    // Send message if there are new upcoming tournaments
                    if (embed.Fields.Count > 0)
                    {
                        await channel.SendMessageAsync(embed: embed.Build());
                    }

                    // Update seen tournaments
                    settings.SeenTournaments = tournamentIds;
                    settings.Save();

                    // Update "Upcoming Tournaments" Message
                    if (!settings.ClashPinnedMessage.HasValue)
                        continue;
                    if (!(await channel.GetMessageAsync(settings.ClashPinnedMessage.Value) is IUserMessage message))
                        continue;
                    if (message.Author.Id != _client.CurrentUser.Id)
                    {
                        _logger.LogInformation("Pinned message about upcoming tournaments is not mine.");
                        continue;
                    }

                    await message.ModifyAsync(m => m.Embed = CreateTournamentListEmbed(TitleUpcomingTournaments).Build());
                }
            }
        }
    }
}