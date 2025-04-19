namespace CdcDataSyncPrototype.Web.Models;

public class PublicationStagingEntry
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime PublishedDate { get; set; }
    public DateTime LastModified { get; set; }
    public int Operation { get; set; }

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public bool Processed { get; set; } = false;
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }
}