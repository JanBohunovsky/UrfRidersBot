using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace UrfRidersBot.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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