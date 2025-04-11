namespace CdcDataSyncPrototype.BusinessApp.Models;

public class Publication
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Type { get; set; } = default!; // e.g. "Report", "Written Evidence", etc.
    public DateTime PublishedDate { get; set; }
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}