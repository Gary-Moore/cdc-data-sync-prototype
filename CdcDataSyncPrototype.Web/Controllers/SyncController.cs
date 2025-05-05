using CdcDataSyncPrototype.Core.Models;
using CdcDataSyncPrototype.Web.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CdcDataSyncPrototype.Web.Controllers
{
    public class SyncController(ReceiverDbContext db, ILogger<SyncController> logger, IConfiguration configuration) : Controller
    {
        private const string ReceiverDbKey = "ReceiverDb";


        public async Task<IActionResult> Publications()
        {
            var publications = await db.Publications
                .OrderByDescending(p => p.LastModified)
                .Take(100)
                .Select(p => new PublicationViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Type = p.Type,
                    PublishedDate = p.PublishedDate,
                    LastModified = p.LastModified
                })
                .ToListAsync();

            return View(publications);
        }

        public async Task<IActionResult> Inbox()
        {
            var entries = await db.SyncInbox
                .OrderByDescending(x => x.ReceivedAt)
                .Take(100)
                .Select(x => new InboxEntryViewModel
                {
                    Id = x.Id,
                    MessageId = x.MessageId,
                    ReceivedAt = x.ReceivedAt,
                    Processed = x.Processed,
                    ProcessedAt = x.ProcessedAt,
                    ErrorMessage = x.ErrorMessage
                })
                .ToListAsync();

            return View(entries);
        }

        [HttpPost]
        public async Task<IActionResult> Retry(Guid id)
        {
            var entry = await db.SyncInbox.FindAsync(id);

            if (entry is not null && entry is { Processed: true, ErrorMessage: not null })
            {
                entry.Processed = false;
                entry.ProcessedAt = null;
                entry.ErrorMessage = null;

                await db.SaveChangesAsync();

                TempData["Message"] = $"Retry queued for message: {entry.MessageId}";
            }

            return RedirectToAction("Inbox");
        }

        public async Task<IActionResult> Operations()
        {
            await using var connection = new SqlConnection(configuration.GetConnectionString(ReceiverDbKey));

            var sql = "SELECT TOP 50 * FROM dbo.Publications_Staging ORDER BY ReceivedAt DESC";
            var entries = (await connection.QueryAsync<PublicationStagingEntry>(sql)).ToList();

            return View(entries);
        }

        [HttpPost]
        public async Task<IActionResult> ApplySync()
        {
            try
            {
                await using var connection = new SqlConnection(configuration.GetConnectionString(ReceiverDbKey));
                await connection.ExecuteAsync("EXEC dbo.sp_ApplyPublicationsFromStaging");

                TempData["Message"] = "Sync applied successfully.";
                logger.LogInformation("Sync procedure executed successfully at {Time}", DateTime.UtcNow);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to apply sync");
                TempData["Message"] = "Sync failed. Check logs.";
            }

            return RedirectToAction("Operations");
        }
    }
}
