namespace CdcDataSyncPrototype.Web.ViewModels;

public class InboxEntryViewModel
{
    public Guid Id { get; set; }
    public string MessageId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }

    public string Status => ErrorMessage != null ? "Error"
        : Processed ? "Processed"
        : "Pending";

    public string StatusClass => ErrorMessage != null ? "text-danger"
        : Processed ? "text-success"
        : "text-warning";

    public string ReceivedAtFormatted => ReceivedAt.ToString("g");
}