using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Infrastructure;

namespace UrfRidersBot.Commands.Public
{
    public class Info : ICommand
    {
        private readonly IBotInformationService _botInfo;
        private readonly IHostEnvironment _hostEnvironment;

        public bool Ephemeral => false;
        public string Name => "info";
        public string Description => "Gets bot's information like uptime, version, etc.";

        public Info(IBotInformationService botInfo, IHostEnvironment hostEnvironment)
        {
            _botInfo = botInfo;
            _hostEnvironment = hostEnvironment;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var embed = EmbedHelper.CreateBotInfo(context.Client);
            embed.AddField("Uptime", _botInfo.Uptime.ToPrettyString());
            embed.AddField("Environment", _hostEnvironment.EnvironmentName);
            embed.AddField("Host", Environment.MachineName);
            embed.AddField(".NET", Environment.Version.ToString(3));

            await context.RespondAsync(embed);
            
            return CommandResult.NoAction;
        }
    }
}