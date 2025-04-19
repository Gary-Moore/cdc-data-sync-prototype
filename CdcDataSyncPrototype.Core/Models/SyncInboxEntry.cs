namespace CdcDataSyncPrototype.Core.Models;
public class SyncInboxEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MessageId { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public bool Processed { get; set; } = false;
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }
}