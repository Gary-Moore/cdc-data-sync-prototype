namespace CdcDataSyncPrototype.Core.Models;
public class PublicationChange
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Type { get; set; } = "";
    public DateTime PublishedDate { get; set; }
    public DateTime LastModified { get; set; }
    public int Operation { get; set; }
}
