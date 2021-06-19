using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrfRidersBot.Common.Services
{
    public class DummyService : IHostedService
    {
        private readonly ILogger<DummyService> _logger;

        public DummyService(ILogger<DummyService> logger)
        {
            _logger = logger;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting...");

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                _logger.LogDebug("Started");

                await Task.Delay(TimeSpan.FromMilliseconds(250));
                _logger.LogTrace("Example trace message");
                
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                _logger.LogDebug("Example debug message");
                
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                _logger.LogInformation("Example information message");
                
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                _logger.LogWarning("Example warning message");
                
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                _logger.LogError("Example error message");
                
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                _logger.LogCritical("Example critical message");
                
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                _logger.LogDebug("Example message with values: (c) {Year} - {Team}", DateTimeOffset.Now.Year, "UrfRiders");
            });
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopped");
            
            return Task.CompletedTask;
        }
    }
}