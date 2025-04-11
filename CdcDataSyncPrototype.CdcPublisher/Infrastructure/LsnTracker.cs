using CdcDataSyncPrototype.CdcPublisher.Services;
using Microsoft.Data.SqlClient;

namespace CdcDataSyncPrototype.CdcPublisher.Infrastructure;

public class LsnTracker(string connectionString) : ILsnTracker
{
    public async Task<byte[]> GetCurrentMaxLsnAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT sys.fn_cdc_get_max_lsn()";

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = new SqlCommand(sql, conn);
        return (byte[])await cmd.ExecuteScalarAsync(cancellationToken);
    }

    public async Task<byte[]> GetLastLsnAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT LastProcessedLsn FROM dbo.CdcSyncCheckpoints WHERE Id = 1";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        return (byte[]) await command.ExecuteScalarAsync(cancellationToken);
    }

    public async Task SaveLastLsnAsync(byte[] lsn, CancellationToken cancellationToken)
    {
        const string sql = "UPDATE dbo.CdcSyncCheckpoints SET LastProcessedLsn = @lsn WHERE Id = 1";

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@lsn", lsn);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}