namespace CdcDataSyncPrototype.CdcPublisher.Services;

public interface ILsnTracker
{
    Task<byte[]> GetLastLsnAsync(CancellationToken cancellationToken);
    Task<byte[]> GetCurrentMaxLsnAsync(CancellationToken cancellationToken);
    Task SaveLastLsnAsync(byte[] lsn, CancellationToken cancellationToken);
}