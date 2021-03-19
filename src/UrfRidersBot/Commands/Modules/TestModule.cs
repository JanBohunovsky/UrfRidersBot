using System.Threading.Tasks;
using Qmmands;
using UrfRidersBot.Infrastructure;

namespace UrfRidersBot.Commands.Modules
{
    public class TestModule : ModuleBase<UrfRidersCommandContext>
    {
        [Command("test")]
        [Description("Just simple test command.")]
        public async Task Test()
        {
            var embed = EmbedHelper
                .CreateSuccess("Test successful! :+1:")
                .AddField("Prefix", Context.Prefix, true)
                .AddField("Member", Context.Member?.Mention ?? "null", true);
            
            await Context.RespondAsync(embed);
        }
    }
}