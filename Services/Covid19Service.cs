using Discord.WebSocket;
using HtmlAgilityPack;
using LiteDB;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using UrfRiders.Data;

namespace UrfRiders.Services
{
    public class Covid19Service
    {
        public Covid19Data CachedData
        {
            get => _cachedData;
            set
            {
                _cachedData = value;
                _collection.Upsert(0, _cachedData);
            }
        }

        private readonly DiscordSocketClient _client;
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<Covid19Data> _collection;
        private readonly ILogger _logger;
        private readonly HttpClient _http;

        private bool _init;
        private Covid19Data _cachedData;

        public Covid19Service(DiscordSocketClient client, LiteDatabase database, ILogger<Covid19Service> logger,
            HttpClient http)
        {
            _client = client;
            _database = database;
            _logger = logger;
            _http = http;

            _collection = _database.GetCollection<Covid19Data>();
            _cachedData = _collection.FindById(0);

            // Run periodic update when the client is ready
            _client.Ready += () =>
            {
                if (!_init)
                {
                    Task.Run(PeriodicUpdate);
                    _init = true;
                }
                return Task.CompletedTask;
            };
        }

        public async Task<Covid19Data> GetLatestData()
        {
            var data = new Covid19Data();
            var html = new HtmlDocument();

            // Download and parse data
            try
            {
                var content = await _http.GetStringAsync("https://onemocneni-aktualne.mzcr.cz/covid-19");
                html.LoadHtml(content);

                // Get numbers
                data.Tests = int.Parse(html.GetElementbyId("count-test").InnerText.Replace(" ", ""));
                data.Sick = int.Parse(html.GetElementbyId("count-sick").InnerText.Replace(" ", ""));
                data.Recovered = int.Parse(html.GetElementbyId("count-recover").InnerText.Replace(" ", ""));
                data.Deaths = int.Parse(html.GetElementbyId("count-dead").InnerText.Replace(" ", ""));

                // Find last update time
                var updateFields = new[]
                {
                    "//*[@id=\"last-modified-tests\"]",
                    "//*[@id=\"last-modified-datetime\"]",
                    "//*[@id=\"prehled\"]/div[3]/div/p[3]",
                    "//*[@id=\"prehled\"]/div[4]/div/p[3]"
                };

                foreach (var xpath in updateFields)
                {
                    var text = html.DocumentNode.SelectSingleNode(xpath).InnerText.Replace("&nbsp;", " ");
                    var time = DateTimeOffset.ParseExact(text, "\\k d. M. yyyy \\v H.mm \\h", CultureInfo.InvariantCulture);
                    if (time > data.LastUpdateTime)
                        data.LastUpdateTime = time;
                }

                if (data.LastUpdateTime == DateTimeOffset.MinValue)
                    throw new Exception("No valid time found.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return data;
        }

        private async Task PeriodicUpdate()
        {
            // How often should we wait (in minutes, max 60)
            const int minutes = 15;
            while (true)
            {
                var now = DateTimeOffset.Now;

                // Ignore seconds and milliseconds
                var target = now.Subtract(TimeSpan.FromSeconds(now.Second))
                    .Subtract(TimeSpan.FromMilliseconds(now.Millisecond));

                // Waits every x minutes starting from full hour
                target = target.AddMinutes(minutes - (target.Minute % minutes));
                _logger.LogDebug($"Next update check at {target:T}");
                await Task.Delay(target - now);

                // Download data
                var data = await GetLatestData();
                if (data == null)
                {
                    _logger.LogDebug("Couldn't download data.");
                    continue;
                }

                // Continue if there is new data (cache is null or newer update time)
                if (CachedData != null && data.LastUpdateTime <= CachedData.LastUpdateTime)
                {
                    _logger.LogDebug($"No new update ({data}).");
                    continue;
                }

                _logger.LogDebug($"New data: {data}");

                // Send message to dedicated channels
                var embed = Covid19Data.CreateEmbed(data, CachedData ?? data).Build();
                foreach (var settings in ServerSettings.All(_client, _database))
                {
                    if (settings.Covid19Channel != null &&
                        _client.GetChannel(settings.Covid19Channel.Value) is SocketTextChannel channel)
                    {
                        await PostUpdate(channel, embed);
                    }
                }

                // Update cache and database
                CachedData = data;
            }
        }

        private async Task PostUpdate(SocketTextChannel channel, Embed embed)
        {
            var message = await MessageToUpdate(channel);
            if (message != null)
                await message.ModifyAsync(m => m.Embed = embed);
            else
                await channel.SendMessageAsync(embed: embed);
        }

        /// <summary>
        /// If the last message in the channel is posted by this service, then it returns that message so it can be updated.
        /// </summary>
        private async Task<IUserMessage> MessageToUpdate(SocketTextChannel channel)
        {
            var lastMessageRaw = (await channel.GetMessagesAsync(1).FlattenAsync()).First();
            if (lastMessageRaw == null)
                return null;
            if (!(lastMessageRaw is IUserMessage lastMessage))
                return null;
            if (lastMessage.Author.Id != _client.CurrentUser.Id)
                return null;
            if (lastMessage.Embeds.Count < 1)
                return null;
            return lastMessage.Embeds.First().Title == Covid19Data.Title ? lastMessage : null;
        }
    }
}