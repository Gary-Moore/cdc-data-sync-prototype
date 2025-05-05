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
                new() {
                    Title = "Select Committee Report 2024",
                    Type = "Report",
                    PublishStartDate = DateTime.UtcNow.AddDays(-30),
                    InternalOnly = false
                },
                new() {
                    Title = "Internal Review Notes",
                    Type = "Internal Memo",
                    PublishStartDate = DateTime.UtcNow.AddDays(-10),
                    InternalOnly = true
                },
                new() {
                    Title = "Future Briefing",
                    Type = "Briefing",
                    PublishStartDate = DateTime.UtcNow.AddDays(+7),
                    InternalOnly = false
                },
                new() {
                    Title = "Archived Evidence Submission",
                    Type = "Evidence",
                    PublishStartDate = DateTime.UtcNow.AddDays(-90),
                    PublishEndDate = DateTime.UtcNow.AddDays(-1),
                    InternalOnly = false
                },
                new() {
                    Title = "Urgent Summary for Media",
                    Type = "Summary",
                    PublishStartDate = DateTime.UtcNow.AddMinutes(-1),
                    InternalOnly = false
                }
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