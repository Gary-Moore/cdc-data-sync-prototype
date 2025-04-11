namespace CdcDataSyncPrototype.BusinessApp.Models;

public class CdcSyncCheckpoint
{
    public int Id { get; set; }
    public byte[] LastProcessedLsn { get; set; } = [];
    public DateTime LastUpdatedUtc { get; set; }
}