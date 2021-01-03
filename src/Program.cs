using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace UrfRidersBot
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
                await host.RunAsync(_hostCts.Token);
                
            } while (_shouldRestart);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddDiscord()
                        .AddConfiguration()
                        .AddServices()
                        .AddUrfRidersDbContext(context.Configuration.GetConnectionString("Postgres"));
                })
                .UseSerilog((context, provider, logger) =>
                {
                    logger
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console();
                    
                    if (context.HostingEnvironment.IsProduction())
                    {
                        var columnWriters = new Dictionary<string, ColumnWriterBase>
                        {
                            { "timestamp", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
                            { "level", new LevelColumnWriter() },
                            { "level_name", new LevelColumnWriter(true, NpgsqlDbType.Text) },
                            { "message", new RenderedMessageColumnWriter() },
                            { "message_template", new MessageTemplateColumnWriter() },
                            { "properties", new PropertiesColumnWriter() },
                            { "exception", new ExceptionColumnWriter() },
                        };

                        logger.WriteTo.PostgreSQL(
                            context.Configuration.GetConnectionString("Postgres"),
                            "log",
                            columnWriters,
                            useCopy: false,
                            needAutoCreateTable: true
                        );
                    }
                });
        }
    }
}