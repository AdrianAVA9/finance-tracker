using System;
using System.Threading;
using System.Threading.Tasks;
using Fintrack.Server.Application.Abstractions.Clock;
using Fintrack.Server.Application.Budgets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fintrack.Server.Infrastructure.BackgroundJobs;

public class RecurringBudgetProcessorJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<RecurringBudgetProcessorJob> _logger;

    public RecurringBudgetProcessorJob(
        IServiceScopeFactory scopeFactory,
        IDateTimeProvider clock,
        ILogger<RecurringBudgetProcessorJob> logger)
    {
        _scopeFactory = scopeFactory;
        _clock = clock;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RecurringBudgetProcessorJob started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_clock.UtcNow.Day == 1)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IRecurringBudgetRollForwardService>();
                    await processor.ProcessAsync(_clock.UtcNow, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred processing recurring budgets.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
