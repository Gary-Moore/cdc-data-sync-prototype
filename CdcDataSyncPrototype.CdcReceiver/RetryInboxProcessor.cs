using CdcDataSyncPrototype.Core.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace CdcDataSyncPrototype.CdcReceiver;

public class RetryInboxProcessor : BackgroundService
{
    private readonly ILogger<RetryInboxProcessor> _logger;
    private readonly IConfiguration _configuration;

    public RetryInboxProcessor(ILogger<RetryInboxProcessor> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var connStr = _configuration.GetConnectionString("ReceiverDb");

                using var connection = new SqlConnection(connStr);

                const string resetSql = @"
                    UPDATE dbo.SyncInbox
                    SET 
                        Processed = 0,
                        ProcessedAt = NULL,
                        ErrorMessage = NULL
                    WHERE 
                        Processed = 1 AND 
                        ErrorMessage IS NOT NULL";

                var affected = await connection.ExecuteAsync(resetSql);

                if (affected > 0)
                {
                    _logger.LogInformation("Auto-retry: Reset {Count} failed entries at {Now}", affected, DateTime.UtcNow);
                }
                else
                {
                    _logger.LogDebug("Auto-retry scan complete. No failed entries found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RetryInboxProcessor failed during execution.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}