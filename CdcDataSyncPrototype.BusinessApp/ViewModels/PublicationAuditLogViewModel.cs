namespace CdcDataSyncPrototype.BusinessApp.ViewModels
{
    public class PublicationAuditLogViewModel
    {
        public int PublicationId { get; set; }
        public string Title { get; set; }
        public string RuleOutcome { get; set; }
        public string? MetadataJson { get; set; }
        public DateTime Timestamp { get; set; }

        public string TimestampAgo => $"{(DateTime.UtcNow - Timestamp).TotalMinutes:F0} mins ago";
    }

}
