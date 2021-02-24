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
                            context.Configuration.GetConnectionString("UrfRidersData"),
                            "log",
                            columnWriters,
                            useCopy: false,
                            needAutoCreateTable: true
                        );
                    }
                });
    }
}