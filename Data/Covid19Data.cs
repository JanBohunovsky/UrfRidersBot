using Discord;
using System;
using System.Globalization;

namespace UrfRiders.Data
{
    public class Covid19Data
    {
        public const string Title = "Aktuální počty onemocnění koronavirem v ČR";

        public int Tests { get; set; }
        public int SickTotal { get; set; }
        public int SickActive { get; set; }
        public int Hospitalized { get; set; }
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
                FormatNumber(data.Tests),
                FormatNumber(data.SickTotal),
                FormatNumber(data.SickActive),
                FormatNumber(data.Hospitalized),
                FormatNumber(data.Recovered),
                FormatNumber(data.Deaths),
                data.LastUpdateTime
            );
        }

        public static EmbedBuilder CreateEmbed(Covid19Data latestData, Covid19Data oldData)
        {
            if (latestData == oldData || oldData == null)
                return CreateEmbed(latestData);

            var tests = FormatNumber(latestData.Tests);
            var sickTotal = FormatNumber(latestData.SickTotal);
            var sickActive = FormatNumber(latestData.SickActive);
            var hospitalized = FormatNumber(latestData.Hospitalized);
            var recovered = FormatNumber(latestData.Recovered);
            var deaths = FormatNumber(latestData.Deaths);

            if (latestData.SickTotal != oldData.SickTotal)
                sickTotal = $"{sickTotal} ({FormatNumber(latestData.SickTotal - oldData.SickTotal, true)})";
            if (latestData.SickActive != oldData.SickActive)
                sickActive = $"{sickActive} ({FormatNumber(latestData.SickActive - oldData.SickActive, true)})";
            if (latestData.Hospitalized != oldData.Hospitalized)
                hospitalized = $"{hospitalized} ({FormatNumber(latestData.Hospitalized - oldData.Hospitalized, true)})";
            if (latestData.Recovered != oldData.Recovered)
                recovered = $"{recovered} ({FormatNumber(latestData.Recovered - oldData.Recovered, true)})";
            if (latestData.Deaths != oldData.Deaths)
                deaths = $"{deaths} ({FormatNumber(latestData.Deaths - oldData.Deaths, true)})";
            if (latestData.Tests != oldData.Tests)
                tests = $"{tests} ({FormatNumber(latestData.Tests - oldData.Tests, true)})";

            return CreateEmbed(tests, sickTotal, sickActive, hospitalized, recovered, deaths, latestData.LastUpdateTime);
        }

        private static EmbedBuilder CreateEmbed(string tests, string sickTotal, string sickActive, 
            string hospitalized, string recovered, string deaths, DateTimeOffset updateTime)
        {
            return new EmbedBuilder()
                .WithColor(0xd31145)
                .WithAuthor("Onemocnění aktuálně",
                    "https://onemocneni-aktualne.mzcr.cz/images/favicon/favicon-196x196.png",
                    "https://onemocneni-aktualne.mzcr.cz/covid-19")
                .WithTitle(Title)
                .AddField("Celkový počet testů", tests, true)
                .AddField("Celkem onemocněných", sickTotal, true)
                .AddField("Aktuálně onemocněných", sickActive, true)
                .AddField("Aktuálně hospitalizovaných", hospitalized, true)
                .AddField("Počet vyléčených", recovered, true)
                .AddField("Počet úmrtí", deaths, true)
                .WithTimestamp(updateTime);
        }

        private static string FormatNumber(int number, bool showSign = false) => (showSign && number > 0 ? "+" : "") + number.ToString("N0", CultureInfo.GetCultureInfo("cs-cz"));
    }
}