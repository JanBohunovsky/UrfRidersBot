using Serilog;
using Serilog.Configuration;

namespace UrfRidersBot.Common.Logging
{
    // Inspired by: https://github.com/k-boyle/Espeon/tree/v4/src/Logging
    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithClassName(this LoggerEnrichmentConfiguration configuration)
        {
            return configuration.With<ClassNameEnricher>();
        }
    }
}