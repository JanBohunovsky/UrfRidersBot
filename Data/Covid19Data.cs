using System;
using System.Globalization;
using System.Linq;
using Discord;

namespace UrfRiders.Data
{
    public class Covid19Data
    {
        public const string Title = "Aktuální počty onemocnění koronavirem v ČR";

        public int Tests { get; set; }
        public int Sick { get; set; }
        public int Recovered { get; set; }
        public int Deaths { get; set; }
        public DateTimeOffset LastUpdateTime { get; set; }

        public override string ToString()
        {
            return $"COVID-19 Data from {LastUpdateTime}";
        }

        public static EmbedBuilder CreateEmbed(Covid19Data data)
        {
            return CreateEmbed(
                FormatNumber(data.Sick),
                FormatNumber(data.Recovered),
                FormatNumber(data.Deaths),
                FormatNumber(data.Tests),
                data.LastUpdateTime
            );
        }

        public static EmbedBuilder CreateEmbed(Covid19Data latestData, Covid19Data oldData)
        {
            if (latestData == oldData)
                return CreateEmbed(latestData);

            var sick = FormatNumber(latestData.Sick);
            var recovered = FormatNumber(latestData.Recovered);
            var deaths = FormatNumber(latestData.Deaths);
            var tests = FormatNumber(latestData.Tests);

            if (latestData.Sick != oldData.Sick)
                sick = $"{sick} ({FormatNumber(latestData.Sick - oldData.Sick, true)})";
            if (latestData.Recovered != oldData.Recovered)
                recovered = $"{recovered} ({FormatNumber(latestData.Recovered - oldData.Recovered, true)})";
            if (latestData.Deaths != oldData.Deaths)
                deaths = $"{deaths} ({FormatNumber(latestData.Deaths - oldData.Deaths, true)})";
            if (latestData.Tests != oldData.Tests)
                tests = $"{tests} ({FormatNumber(latestData.Tests - oldData.Tests, true)})";

            return CreateEmbed(sick, recovered, deaths, tests, latestData.LastUpdateTime);
        }

        private static EmbedBuilder CreateEmbed(string sick, string recovered, string deaths, string tests, DateTimeOffset updateTime)
        {
            return new EmbedBuilder()
                .WithColor(0xd31145)
                .WithAuthor("Onemocnění aktuálně",
                    "https://onemocneni-aktualne.mzcr.cz/images/favicon/favicon-196x196.png",
                    "https://onemocneni-aktualne.mzcr.cz/covid-19")
                .WithTitle(Title)
                .AddField("Celkový počet osob s prokázanou nákazou COVID-19", sick)
                .AddField("Celkový počet vyléčených", recovered)
                .AddField("Celkový počet úmrtí", deaths)
                .AddField("Celkový počet provedených testů", tests)
                .WithTimestamp(updateTime);
        }

        private static string FormatNumber(int number, bool showSign = false) => (showSign && number > 0 ? "+" : "") + number.ToString("N0", CultureInfo.GetCultureInfo("cs-cz"));
    }
}