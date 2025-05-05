using System.Text.Json;
using CdcDataSyncPrototype.Core.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CdcDataSyncPrototype.CdcReceiver;

public class InboxProcessor : BackgroundService
{
    private readonly ILogger<InboxProcessor> _logger;
    private readonly IConfiguration _configuration;

    public InboxProcessor(ILogger<InboxProcessor> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var connStr = _configuration.GetConnectionString("ReceiverDb");

                using var connection = new SqlConnection(connStr);

                const string loadSql = @"
                    SELECT TOP 10 * 
                    FROM dbo.SyncInbox
                    WHERE Processed = 0
                    ORDER BY ReceivedAt ASC";

                var entries = (await connection.QueryAsync<SyncInboxEntry>(loadSql)).ToList();

                foreach (var entry in entries)
                {
                    try
                    {
                        // Deserialize the payload to the appropriate type
                        var publication = JsonSerializer.Deserialize<PublicationStagingEntry>(entry.Payload);

                        if (publication == null)
                        {
                            throw new Exception("Deserialized payload is null");
                        }

                        const string updateStagingSql = @"
                            MERGE dbo.Publications_Staging AS target
                            USING (SELECT @Id AS Id) AS source
                            ON target.Id = source.Id
                            WHEN NOT MATCHED THEN
                                INSERT (Id, Title, Type, PublishedDate, LastModified, Operation, ReceivedAt)
                                VALUES (@Id, @Title, @Type, @PublishedDate, @LastModified, @Operation, @ReceivedAt);
                        ";

                        await connection.ExecuteAsync(updateStagingSql, new
                        {
                            publication.Id,
                            publication.Title,
                            publication.Type,
                            PublishedDate = publication.PublishStartDate,
                            publication.LastModified,
                            publication.Operation,
                            ReceivedAt = DateTime.UtcNow
                        });

                        // Apply from staging to main table via stored proc
                        await connection.ExecuteAsync("EXEC dbo.sp_ApplyPublicationsFromStaging");

                        // Mark SyncInbox entry as processed
                        const string markProcessedSql = @"
                            UPDATE dbo.SyncInbox
                            SET Processed = 1, ProcessedAt = @Now
                            WHERE Id = @Id";
                        
                        await connection.ExecuteAsync(markProcessedSql, new { Id = entry.Id, Now = DateTime.UtcNow });

                        _logger.LogInformation("Processed and applied inbox message: {Id}", entry.MessageId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process inbox entry: {Id}", entry.Id);

                        const string failSql = @"
                            UPDATE dbo.SyncInbox
                            SET ErrorMessage = @Error
                            WHERE Id = @Id;";

                        await connection.ExecuteAsync(failSql, new { Id = entry.Id, Error = ex.Message });
                    }
                }

                // Wait a bit before next polling cycle
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during inbox processing loop");
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); // Wait before retrying
            }
        }
    }
}