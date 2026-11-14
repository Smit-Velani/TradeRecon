using TradeRecon.Application.Services;

namespace TradeRecon.Api.BackgroundServices;

public class ReconciliationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReconciliationWorker> _logger;
    private readonly IConfiguration _configuration;

    public ReconciliationWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ReconciliationWorker> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var intervalMinutes =
            _configuration.GetValue<int>(
                "ReconciliationWorker:IntervalMinutes",
                15);

        _logger.LogInformation(
            "Reconciliation worker started. Interval={IntervalMinutes} minutes",
            intervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(
                    TimeSpan.FromMinutes(intervalMinutes),
                    stoppingToken);

                using var scope =
                    _scopeFactory.CreateScope();

                var service =
                    scope.ServiceProvider
                        .GetRequiredService<IReconciliationService>();

                var results =
                    await service.ReconcileAsync();

                _logger.LogInformation(
                    "Scheduled reconciliation completed. Results={Count}",
                    results.Count);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Scheduled reconciliation failed.");
            }
        }

        _logger.LogInformation(
            "Reconciliation worker stopped.");
    }
}
