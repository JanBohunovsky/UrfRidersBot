using System.Threading.Tasks;
using Discord.Commands;
using UrfRiders.Data;
using UrfRiders.Services;

namespace UrfRiders.Modules
{
    [Name("COVID-19")]
    public class Covid19Module : BaseModule
    {
        public Covid19Service Service { get; set; }

        [Command("corona")]
        [Alias("korona")]
        public async Task CoronaStats()
        {
            await Context.Channel.TriggerTypingAsync();
            var data = await Service.GetLatestData();
            if (Context.Channel.Id == Settings.Covid19Channel)
            {
                await ReplyAsync(embed: Covid19Data.CreateEmbed(data, Service.CachedData ?? data).Build());
                Service.CachedData = data;
            }
            else
            {
                await ReplyAsync(embed: Covid19Data.CreateEmbed(data).Build());
            }
        }
    }
}