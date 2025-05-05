
namespace CdcDataSyncPrototype.Core.Models;
public class PublicationStagingEntry
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Type { get; set; } = default!;
    public bool InternalOnly { get; set; }
    public DateTime? PublishStartDate { get; set; }
    public DateTime? PublishEndDate { get; set; }
    public DateTime LastModified { get; set; }

    // CDC + staging metadata
    public int Operation { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
