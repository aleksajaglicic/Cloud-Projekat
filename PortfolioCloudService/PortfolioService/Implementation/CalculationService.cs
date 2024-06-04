using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortfolioService.Implementation
{
    public class CalculationService : IHostedService, IDisposable
    {
        private readonly ILogger<CalculationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        private static int _lastTransactionId = 0;

        public CalculationService(ILogger<CalculationService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting the periodic task.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation($"127.0.0.1 - - [{DateTime.Now:dd/MMM/yyyy HH:mm:ss}] \"Updating users portfolios\"");
            using (var scope = _serviceProvider.CreateScope())
            {
                //var portfolioService = scope.ServiceProvider.GetRequiredService<>();
                //portfolioService.CalculatePortfolio();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping the periodic task.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
