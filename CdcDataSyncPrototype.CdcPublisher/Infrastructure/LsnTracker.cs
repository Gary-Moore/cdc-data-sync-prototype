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

    public async Task<byte[]?> GetLastLsnAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT LastProcessedLsn FROM dbo.CdcSyncCheckpoints WHERE Id = 1";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result is null || result == DBNull.Value)
        {
            return null;
        }

        var lsn = (byte[])result;

        if (lsn.All(b => b == 0))
        {
            return null;
        }

        return lsn;
    }

    public async Task<byte[]> GetMinLsnAsync(string captureInstance, CancellationToken cancellationToken)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT sys.fn_cdc_get_min_lsn(@captureInstance)";
        cmd.Parameters.AddWithValue("@captureInstance", captureInstance);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);

        return result as byte[] ?? throw new InvalidOperationException("Failed to retrieve min LSN.");
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