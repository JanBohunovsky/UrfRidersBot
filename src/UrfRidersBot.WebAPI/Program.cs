using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.PostgreSQL;
using UrfRidersBot.Persistence;

namespace UrfRidersBot.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            // Make sure the database is on the latest migration
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var dbContextFactory = host.Services.GetRequiredService<IDbContextFactory<UrfRidersDbContext>>();
            await using (var dbContext = dbContextFactory.CreateDbContext())
            {
                logger.LogInformation("Migrating database...");
                await dbContext.Database.MigrateAsync();
            }

            await host.RunAsync();
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

            // BaseDiscordClient is spamming Error and Fatal logs which give no value.
            // e.g. [Fata] Connection terminated ... reconnecting -- This happens when Discord requests WebSocket restart (which is normal)
            //                                                       but also when the bot gets disconnected...
            //      [Error] Rate limit hit, re-queueing request to ... -- This should've been a warning, like seriously...
            logger.Filter.ByExcluding(Matching.FromSource("DSharpPlus.BaseDiscordClient"));
            
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
                context.Configuration.GetConnectionString("UrfRidersData"),
                "log",
                columnWriters,
                useCopy: false,
                needAutoCreateTable: true
            );
        }
    }
}