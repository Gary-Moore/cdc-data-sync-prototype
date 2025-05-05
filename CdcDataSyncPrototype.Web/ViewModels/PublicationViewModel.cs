namespace CdcDataSyncPrototype.Web.ViewModels;

public class PublicationViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public DateTime PublishedDate { get; set; }
    public DateTime LastModified { get; set; }

    public string PublishedDateFormatted => PublishedDate.ToString("dd MMM yyyy");
    public string LastModifiedAgo => $"{(DateTime.UtcNow - LastModified).TotalMinutes:F0} mins ago";
}