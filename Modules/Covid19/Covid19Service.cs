﻿using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using LiteDB;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UrfRiders.Modules.Settings;

namespace UrfRiders.Modules.Covid19
{
    public class Covid19Service
    {
        private readonly DiscordSocketClient _client;
        private readonly LiteDatabase _database;
        private readonly ILogger _logger;
        private readonly HttpClient _http;

        private bool _init;
        private Regex _datetimePattern;

        public Covid19Service(DiscordSocketClient client, LiteDatabase database, ILogger<Covid19Service> logger, HttpClient http)
        {
            _client = client;
            _database = database;
            _logger = logger;
            _http = http;

            _datetimePattern = new Regex("(\\d{1,2}\\.) (\\d{1,2}\\.) (\\d{4}) v (\\d{1,2}\\.\\d{1,2})");

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
                data.SickTotal = int.Parse(html.GetElementbyId("count-sick").InnerText.Replace(" ", ""));
                data.SickActive = int.Parse(html.GetElementbyId("count-active").InnerText.Replace(" ", ""));
                data.Hospitalized = int.Parse(html.GetElementbyId("count-hospitalization").InnerText.Replace(" ", ""));
                data.Recovered = int.Parse(html.GetElementbyId("count-recover").InnerText.Replace(" ", ""));
                data.Deaths = int.Parse(html.GetElementbyId("count-dead").InnerText.Replace(" ", ""));

                // Find last update time
                var updateFields = new[]
                {
                    "//*[@id=\"last-modified-tests\"]",
                    "//*[@id=\"last-modified-datetime\"]",
                    "//*[@id=\"prehled\"]/div[3]/div/p[3]",
                    "//*[@id=\"prehled\"]/div[4]/div/p[3]",
                    "//*[@id=\"prehled\"]/div[5]/div/p[3]",
                    "//*[@id=\"prehled\"]/div[6]/div/p[3]",
                };

                foreach (var xpath in updateFields)
                {
                    var rawText = html.DocumentNode.SelectSingleNode(xpath).InnerText.Replace("&nbsp;", " ");
                    var text = _datetimePattern.Match(rawText).ToString();
                    var time = DateTimeOffset.ParseExact(text, "d. M. yyyy \\v H.mm", CultureInfo.InvariantCulture);
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
            // Check for update every X minutes, starting from full hour.
            // Choose X here:
            const int minute = 15;
            while (true)
            {
                var now = DateTimeOffset.Now;
                var target = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 30, now.Offset);

                // Calculate next iteration of `minute`.
                // Example:
                //   now = 9:37
                //   minute = 15
                // 37 / 15 = 2.466 (round up => 3)
                // 3 * 15 = 45
                // Now we know we should wait until it's 9:45.
                target = target.AddMinutes(minute * Math.Ceiling(now.Minute / (double)minute));
                _logger.LogDebug($"Next update check at {target:T}");
                await Task.Delay(target - now);


                // Download data
                var data = await GetLatestData();
                if (data == null)
                {
                    _logger.LogWarning("Couldn't download data.");
                    continue;
                }

                // Send message to dedicated channels
                foreach (var settings in ServerSettings.All(_client, _database))
                {
                    if (!settings.Covid19Channel.HasValue)
                        continue;
                    if (!(_client.GetChannel(settings.Covid19Channel.Value) is SocketTextChannel channel))
                        continue;
                    if (settings.Covid19CachedData != null && data.LastUpdateTime <= settings.Covid19CachedData.LastUpdateTime)
                        continue;

                    var embed = Covid19Data.CreateEmbed(data, settings.Covid19CachedData ?? data).Build();
                    await PostUpdate(channel, embed);
                    settings.Covid19CachedData = data;
                }
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