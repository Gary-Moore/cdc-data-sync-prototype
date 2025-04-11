using CdcDataSyncPrototype.BusinessApp.Models;

namespace CdcDataSyncPrototype.BusinessApp.Data;

public class DbInitializer
{
    public static void SeedData(AppDbContext context)
    {
        if (!context.Publications.Any())
        {
            var publications = new List<Publication>
            {
                new() { Title = "Select Committee Report 2024", Type = "Report", PublishedDate = new DateTime(2024, 11, 1) },
                new() { Title = "Written Evidence Submission A1", Type = "Evidence", PublishedDate = new DateTime(2024, 10, 15) },
                new() { Title = "Budget Briefing Summary", Type = "Briefing", PublishedDate = new DateTime(2024, 12, 5) },
            };

            context.Publications.AddRange(publications);
        }

        if (!context.CdcSyncCheckpoints.Any())
        {
            context.CdcSyncCheckpoints.Add(new CdcSyncCheckpoint()
            {
                Id = 1,
                LastProcessedLsn = new byte[10],
                LastUpdatedUtc = DateTime.UtcNow
            });
        }
        
        context.SaveChanges();
    }
}