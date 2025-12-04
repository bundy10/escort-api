using Escort.Worker.Payout.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Escort.Worker.Payout.Workers;

public class PayoutWorker : BackgroundService
{
    private readonly ILogger<PayoutWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _interval;

    public PayoutWorker(
        ILogger<PayoutWorker> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        
        // Get interval from configuration, default to 60 minutes
        var intervalMinutes = _configuration.GetValue<int>("PayoutWorker:IntervalInMinutes", 60);
        _interval = TimeSpan.FromMinutes(intervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Payout Worker started. Processing interval: {Interval} minutes",
            _interval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Payout Worker running at: {Time}", DateTimeOffset.UtcNow);

                // Create a new scope for the scoped service
                using (var scope = _serviceProvider.CreateScope())
                {
                    var payoutService = scope.ServiceProvider
                        .GetRequiredService<IPayoutProcessingService>();
                    
                    await payoutService.ProcessPendingPayoutsAsync(stoppingToken);
                }

                _logger.LogInformation(
                    "Payout Worker completed. Next run at: {Time}",
                    DateTimeOffset.UtcNow.Add(_interval));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Payout Worker");
            }

            // Wait for the configured interval before next execution
            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Payout Worker stopped");
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payout Worker is starting");
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payout Worker is stopping");
        await base.StopAsync(cancellationToken);
    }
}

