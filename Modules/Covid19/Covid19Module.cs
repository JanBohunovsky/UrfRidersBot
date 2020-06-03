using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace UrfRiders.Modules.Covid19
{
    [Name("COVID-19")]
    public class Covid19Module : BaseModule
    {
        public Covid19Service Service { get; set; }

        [Command("corona")]
        [Alias("korona", "coronavirus", "koronavirus", "covid19")]
        public async Task CoronaStats()
        {
            bool usingCache = false;

            await Context.Channel.TriggerTypingAsync();
            var data = await Service.GetLatestData();
            if (data == null)
            {
                data = Settings.Covid19CachedData;
                usingCache = true;
            }

            EmbedBuilder embedBuilder;
            if (Context.Channel.Id == Settings.Covid19Channel)
            {
                embedBuilder = Covid19Data.CreateEmbed(data, Settings.Covid19CachedData ?? data);
                Settings.Covid19CachedData = data;
            }
            else
            {
                embedBuilder = Covid19Data.CreateEmbed(data);
            }

            if (usingCache)
                embedBuilder.WithFooter("Failed to update, using cached version", "https://u.cubeupload.com/Bohush/iconerror.png");

            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}