using CdcDataSyncPrototype.CdcPublisher.Services;

namespace CdcDataSyncPrototype.CdcPublisher;

public class CdcSyncWorker(
    ILsnTracker lsnTracker,
    ILogger<CdcSyncWorker> logger,
    IConfiguration config)
    : BackgroundService
{
    private readonly string _connectionString = config.GetConnectionString("DefaultConnection");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("CDC Sync Worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var fromLsn = await lsnTracker.GetLastLsnAsync(stoppingToken);
                var toLsn = await lsnTracker.GetCurrentMaxLsnAsync(stoppingToken);

                logger.LogInformation("Polling CDC changes from LSN {FromLsn} to {ToLsn}",
                    BitConverter.ToString(fromLsn), BitConverter.ToString(toLsn));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CDC worker encountered an error");
            }

           

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
