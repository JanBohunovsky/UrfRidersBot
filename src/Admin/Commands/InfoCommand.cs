using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Common.Commands;

namespace UrfRidersBot.Admin.Commands
{
    [Command("info", "Display bot's basic information.", typeof(TestGroup))]
    public class InfoCommand : ICommand
    {
        private readonly IHostEnvironment _environment;

        public InfoCommand(IHostEnvironment environment)
        {
            _environment = environment;
        }
        
        public async Task HandleAsync(CommandContext context)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Bot Information")
                .AddField("Application name", _environment.ApplicationName)
                .AddField("Environment", _environment.EnvironmentName)
                .AddField("Host name", Environment.MachineName)
                .AddField("OS version", Environment.OSVersion.ToString())
                .AddField(".NET version", Environment.Version.ToString(3));

            await context.RespondAsync(embed);
        }
    }
}