using System;
using System.Threading.Tasks;
using LiteDB.Async;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace UrfRidersBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var database = host.Services.GetRequiredService<ILiteDatabaseAsync>();
            
            await host.RunAsync();
            
            // TODO: Improve when we introduce domain events
            // Since we're using the LiteDatabaseAsync as a singleton (which creates a background thread),
            // it doesn't get disposed and blocks the application from closing (because of the background thread).
            //
            // I've tried IHostedService but that doesn't work great (throws an exception).
            // It could be possible to wrap this somehow but we cannot get the instance AFTER the host is finished,
            // because the IServiceProvider would already be disposed.
            //
            // Maybe we can use domain events (like Mediator) to raise "ApplicationClosing" event here
            // and handle it in some handler (where it can get the database instance in ctor).
            // But I don't want to introduce it just for this small thing.
            database.Dispose();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog(ConfigureSerilog);

        private static void ConfigureSerilog(HostBuilderContext context, IServiceProvider provider,
            LoggerConfiguration logger)
        {
            logger
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            if (!context.HostingEnvironment.IsProduction()) 
                return;

            logger.WriteTo.LiteDB("log.db", restrictedToMinimumLevel: LogEventLevel.Warning);
        }
    }
}