using Application.Interface;
using Microsoft.Extensions.Logging;
using Quartz;

public class PlaidSyncJob : IJob
{
    private readonly IPlaidSyncService _plaidSyncService;
    private readonly ILogger<PlaidSyncJob> _logger;

    public PlaidSyncJob(IPlaidSyncService plaidSyncService, ILogger<PlaidSyncJob> logger)
    {
        _plaidSyncService = plaidSyncService ?? throw new ArgumentNullException(nameof(plaidSyncService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("PlaidSyncJob execution started.");
        try
        {
            //await _plaidSyncService.RunDailySync();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during PlaidSyncJob execution: {ex.Message}");
        }
        _logger.LogInformation("PlaidSyncJob execution completed.");
    }
}