using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;
using Serilog;
using UrfRidersBot.Library;

namespace UrfRidersBot.ConsoleUI
{
    internal static class Program
    {
        private static CancellationTokenSource _hostCts = null!;
        private static bool _shouldRestart = false;

        public static void Shutdown(TimeSpan? delay = null)
        {
            StopApplication(delay, false);
        }

        public static void Restart(TimeSpan? delay = null)
        {
            StopApplication(delay, true);
        }

        private static void StopApplication(TimeSpan? delay, bool shouldRestart)
        {
            _shouldRestart = shouldRestart;
            
            if (delay != null)
                _hostCts.CancelAfter(delay.Value);
            else
                _hostCts.Cancel();
        }
        
        private static async Task Main(string[] args)
        {
            do
            {
                _hostCts = new CancellationTokenSource();
                _shouldRestart = false;
                
                var host = CreateHostBuilder(args).Build();
                await host.InitializeServices(Assembly.GetEntryAssembly());
                await host.RunAsync(_hostCts.Token);
                
            } while (_shouldRestart);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddDiscord();
                    services.AddUrfRidersBot(hostingContext.Configuration);
                })
                .UseSerilog((hostingContext, serviceProvider, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console();
                    
                    if (hostingContext.HostingEnvironment.IsProduction())
                    {
                        loggerConfiguration.WriteTo.RavenDB(serviceProvider.GetRequiredService<IDocumentStore>());
                    }
                });
        }
    }
}