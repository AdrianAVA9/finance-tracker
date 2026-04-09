using Fintrack.Server.Application.Expenses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fintrack.Server.Infrastructure.BackgroundJobs;

/// <summary>
/// Polls for queued receipt uploads and processes them with AI (same pipeline as synchronous <c>/receipts/process</c>).
/// </summary>
public sealed class PendingReceiptExtractionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PendingReceiptExtractionBackgroundService> _logger;
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(3);

    public PendingReceiptExtractionBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<PendingReceiptExtractionBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PendingReceiptExtractionBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<PendingReceiptExtractionProcessor>();
                await processor.ProcessNextIfAnyAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in pending receipt extraction loop.");
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
