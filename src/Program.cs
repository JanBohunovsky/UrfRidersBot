using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using UrfRidersBot.Common;
using UrfRidersBot.Common.Configuration;
using UrfRidersBot.Common.Services;

namespace UrfRidersBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((context, logger) =>
                {
                    logger.ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureDiscord()
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;
                    
                    services.AddHostedService<DiscordService>()
                        .AddHostedService<DummyService>()
                        .ConfigureSection<Discord>(configuration);
                });
        }
    }
}