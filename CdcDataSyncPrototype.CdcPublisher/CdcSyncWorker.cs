using System.Data;
using System.Text.Json;
using CdcDataSyncPrototype.CdcPublisher.Services;
using CdcDataSyncPrototype.Core.Models;
using Microsoft.Data.SqlClient;

namespace CdcDataSyncPrototype.CdcPublisher;

public class CdcSyncWorker(
    ILsnTracker lsnTracker,
    IAzureServiceBusPublisher publisher,
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
                var fromLsn = await lsnTracker.GetLastLsnAsync(stoppingToken)
                            ?? await lsnTracker.GetMinLsnAsync("dbo_Publications", stoppingToken);

                var toLsn = await lsnTracker.GetCurrentMaxLsnAsync(stoppingToken);

                logger.LogInformation("Polling CDC changes from LSN {FromLsn} to {ToLsn}",
                    BitConverter.ToString(fromLsn), BitConverter.ToString(toLsn));

                var changes = await GetCdcChangesAsync(fromLsn, toLsn, stoppingToken);

                // Optional: Filter out before-image rows (Op 1 & 3)
                changes = changes
                    .Where(c => c.Operation is 1 or 2 or 4)
                    .ToList();

                foreach (var change in changes)
                {
                    var json = JsonSerializer.Serialize(change);

                    await publisher.PublishMessageAsync(json, subject: "cdc.publications", stoppingToken);

                    logger.LogInformation("📨 Published {Op} message for Id={Id} | Title='{Title}'",
                        GetOperationName(change.Operation), change.Id, change.Title);
                }

                // Only checkpoint if we got to this point cleanly
                await lsnTracker.SaveLastLsnAsync(toLsn, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CDC worker encountered an error");
            }

            var delaySeconds = config.GetValue<int>("CdcPolling:IntervalSeconds", 10);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
        }
    }

    private async Task<List<PublicationChange>> GetCdcChangesAsync(byte[] fromLsn, byte[] toLsn, CancellationToken stoppingToken)
    {
        var changes = new List<PublicationChange>();

        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(stoppingToken);

        var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT * 
            FROM cdc.fn_cdc_get_all_changes_dbo_Publications(@from_lsn, @to_lsn, N'all');";

        cmd.Parameters.Add(new SqlParameter("@from_lsn", SqlDbType.Binary, 10) { Value = fromLsn });
        cmd.Parameters.Add(new SqlParameter("@to_lsn", SqlDbType.Binary, 10) { Value = toLsn });

        using var reader = await cmd.ExecuteReaderAsync(stoppingToken);

        while (await reader.ReadAsync(stoppingToken))
        {
            var change = new PublicationChange
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Type = reader.GetString(reader.GetOrdinal("Type")),
                PublishedDate = reader.GetDateTime(reader.GetOrdinal("PublishedDate")),
                LastModified = reader.GetDateTime(reader.GetOrdinal("LastModified")),
                Operation = reader.GetInt32(reader.GetOrdinal("__$operation"))
            };

            changes.Add(change);
        }

        return changes;
    }

    private static string GetOperationName(int op) => op switch
    {
        1 => "DELETE (before image)",
        2 => "INSERT",
        3 => "UPDATE (before image)",
        4 => "UPDATE (after image)",
        _ => $"UNKNOWN ({op})"
    };
}