using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UrfRidersBot.Library
{
    public static class HostExtensions
    {
        public static async Task InitializeServices(this IHost host, Assembly? entryAssembly)
        {
            var commandHandlingService = host.Services.GetRequiredService<CommandHandlingService>();
            await commandHandlingService.RegisterCommands(entryAssembly);

            host.Services.GetRequiredService<DiscordLogService>();
        }
    }
}