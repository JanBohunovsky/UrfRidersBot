using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
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
        public const string TitleThisWeekTournaments = "Tournaments this week";
        public const string DescriptionTournamentToday = "Today's tournament:";
        public const string DescriptionNoTournaments = "No planned tournaments.";

        public ClashTournamentData[] Tournaments { get; private set; }

        public EmbedBuilder BaseEmbed => new EmbedBuilder()
            .WithColor(Color.Blue)
            .WithThumbnailUrl("https://cdn.discordapp.com/attachments/717788228899307551/717788431006302218/Clash_Crest_icon.webp");

        // When does a day start.
        private readonly TimeSpan _newDay = new TimeSpan(6, 0, 0);
        // When should service notify servers.
        private readonly TimeSpan _morning = new TimeSpan(8, 0, 0);
        private readonly TimeSpan _evening = new TimeSpan(22, 0, 0);

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

                // API key is ok, start periodic update
                _ = Task.Run(PeriodicUpdate);

            };
        }

        #region CreateTournamentListEmbed

        public EmbedBuilder CreateTournamentListEmbed(string title, IEnumerable<ClashTournamentData> tournaments)
        {
            var embedBuilder = BaseEmbed.WithTitle(title);
            
            foreach (var tournament in tournaments)
            {
                embedBuilder.AddField(tournament.FormattedTime, tournament.FormattedName);
            }

            return embedBuilder;
        }

        public EmbedBuilder CreateTournamentListEmbed(string title, Func<ClashTournamentData, bool> predicate) =>
            CreateTournamentListEmbed(title, Tournaments.Where(predicate));

        public EmbedBuilder CreateTournamentListEmbed(string title) => CreateTournamentListEmbed(title, Tournaments);

        #endregion

        public async Task UpdateChannelTopic(SocketTextChannel channel)
        {
            if (channel == null)
                return;

            // Get the first upcoming tournament (`Tournaments` array is sorted by registration time).
            var now = DateTimeOffset.Now;
            var nextTournament = Tournaments.FirstOrDefault(t => t.StartTime > now);
            if (nextTournament == null)
            {
                if (channel.Topic != DescriptionNoTournaments)
                    await channel.ModifyAsync(c => c.Topic = DescriptionNoTournaments);
                return;
            }

            // Calculate remaining days.
            var start = nextTournament.StartTime;
            var target = new DateTimeOffset(start.Year, start.Month, start.Day, _newDay.Hours, _newDay.Minutes, _newDay.Seconds, start.Offset);
            var countdown = target - now;

            var countdownText = target.ToString("M");
            if (countdown.TotalDays <= 0)
                countdownText = "Today";
            else if (countdown.TotalDays <= 1)
                countdownText = "Tomorrow";
            else if (countdown.TotalDays <= 14)
                countdownText = $"In {countdown.Days + 1} days";

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
        /// Downloads tournaments every full hour, then notifies servers about new tournaments available.
        /// Every day at <see cref="_newDay"/> updates channel topics to update countdown.
        /// Every day at <see cref="_morning"/> posts notifications about upcoming tournaments (if valid, i.e. at the start of the week and the day of tournament)
        /// </summary>
        private async Task PeriodicUpdate()
        {
            while (true)
            {
                var now = DateTimeOffset.Now;
                var target = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 30, now.Offset);

                // Calculate target (next full hour)
                target = target.AddHours(1);
                _logger.LogDebug($"Next download at {target:T}");
                await Task.Delay(target - now);

                now = DateTimeOffset.Now;
                
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

                if (now.Hour >= _morning.Hours && now.Hour <= _evening.Hours)
                    await NotifyServersAboutNewTournaments();
                if (now.Hour == _newDay.Hours)
                    await NewDayUpdate();
                if (now.Hour == _morning.Hours)
                    await MorningNotifications();
            }
        }

        private async Task NotifyServersAboutNewTournaments()
        {
            // Notify servers about new tournaments that are available
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

        private async Task NewDayUpdate()
        {
            // At the start of the day, update channel topics
            foreach (var settings in ServerSettings.All(_client, _database))
            {
                if (!settings.ClashChannel.HasValue)
                    continue;
                if (!(_client.GetChannel(settings.ClashChannel.Value) is SocketTextChannel channel))
                    continue;

                await UpdateChannelTopic(channel);
            }
        }

        private async Task MorningNotifications()
        {
            // Notify servers about upcoming tournaments that are happening this week and today.
            var now = DateTimeOffset.Now;
            Embed embedThisWeek = null, embedToday = null;
            if (now.DayOfWeek == DayOfWeek.Monday)
            {
                var start = now;
                var end = now.AddDays(6);
                var tournamentsThisWeek = Tournaments.Where(t => t.StartTime.Date >= start.Date && t.StartTime.Date <= end.Date).ToArray();

                if (tournamentsThisWeek.Length > 0)
                    embedThisWeek = CreateTournamentListEmbed(TitleThisWeekTournaments, tournamentsThisWeek).Build();
            }

            var tournamentToday = Tournaments.FirstOrDefault(t => t.StartTime.Date == now.Date);
            if (tournamentToday != null)
            {
                var description = new StringBuilder();
                description.AppendLine($"Registration at **{tournamentToday.RegistrationTime:h\\:mm tt}**");
                description.AppendLine($"Start at **{tournamentToday.StartTime:h\\:mm tt}**");
                embedToday = BaseEmbed.WithTitle(tournamentToday.FormattedName).WithDescription(description.ToString()).Build();
            }

            foreach (var settings in ServerSettings.All(_client, _database))
            {
                if (!settings.ClashChannel.HasValue)
                    continue;
                if (!(_client.GetChannel(settings.ClashChannel.Value) is SocketTextChannel channel))
                    continue;

                if (embedThisWeek != null)
                {
                    await channel.SendMessageAsync(embed: embedThisWeek);
                }

                if (embedToday != null)
                {
                    await channel.SendMessageAsync(DescriptionTournamentToday, embed: embedToday);
                }
            }
        }
    }
}