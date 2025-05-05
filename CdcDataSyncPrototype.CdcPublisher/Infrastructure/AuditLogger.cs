using CdcDataSyncPrototype.CdcPublisher.Services;
using CdcDataSyncPrototype.Core.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace CdcDataSyncPrototype.CdcPublisher.Infrastructure;

public class AuditLogger : IAuditLogger
{
    private readonly string _connectionString;

    public AuditLogger(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SyncDb")
                            ?? throw new InvalidOperationException("Missing SyncDb connection string");
    }

    public async Task LogAsync(PublicationAuditLog entry, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO dbo.PublicationAuditLog
            (Id, PublicationId, Title, RuleOutcome, Timestamp)
            VALUES (@Id, @PublicationId, @Title, @RuleOutcome, @Timestamp);
        ";

        await using var conn = new SqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new
        {
            entry.Id,
            entry.PublicationId,
            entry.Title,
            entry.RuleOutcome,
            entry.Timestamp
        });
    }
}