namespace CdcDataSyncPrototype.Core.Models;
public class PublicationAuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int PublicationId { get; set; }
    public string Title { get; set; }
    public string RuleOutcome { get; set; } // e.g. "Suppressed: InternalOnlyRule"
    public string? MetadataJson { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
