using System.Collections.Concurrent;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace UrfRidersBot.Common.Logging
{
    // Inspired by: https://github.com/k-boyle/Espeon/tree/v4/src/Logging
    public class ClassNameEnricher : ILogEventEnricher
    {
        private const int Padding = 20;

        private readonly ConcurrentDictionary<string, LogEventProperty> _classNameCache;
        
        public ClassNameEnricher()
        {
            _classNameCache = new ConcurrentDictionary<string, LogEventProperty>();
        }
        
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var sourceContext = (string)((ScalarValue)logEvent.Properties["SourceContext"]).Value;
            var property = _classNameCache.GetOrAdd(sourceContext, CreateClassNameProperty);

            logEvent.AddOrUpdateProperty(property);
        }

        private static LogEventProperty CreateClassNameProperty(string sourceContext)
        {
            var split = sourceContext.Split('.');
            
            var sourceClass = split[^1];
            var sourceNamespace = split[..^1];
            var firstNamespace = sourceNamespace[0];

            // If the class isn't from this solution, show the class name with initials of its namespace.
            // e.g. Microsoft.Hosting.Lifetime -> M.H.Lifetime
            if (firstNamespace != typeof(Program).Namespace)
            {
                var shortenedNamespace = string.Join('.', sourceNamespace.Select(name => name[0]));
                sourceClass = string.Concat(shortenedNamespace, ".", sourceClass);
            }

            // Otherwise just show the class name
            sourceClass = sourceClass.Length > Padding
                ? string.Concat(sourceClass.Substring(0, Padding - 3), "...")
                : sourceClass.PadRight(Padding, ' ');

            return new LogEventProperty("ClassName", new ScalarValue(sourceClass));
        }
    }
}