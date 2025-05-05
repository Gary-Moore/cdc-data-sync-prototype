using CdcDataSyncPrototype.BusinessApp.Data;
using CdcDataSyncPrototype.BusinessApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CdcDataSyncPrototype.BusinessApp.Controllers
{
    public class AuditController(AppDbContext db) : Controller
    {
        public async Task<IActionResult> Rules()
        {
            var logs = await db.PublicationAuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .Select(l => new PublicationAuditLogViewModel
                {
                    PublicationId = l.PublicationId,
                    Title = l.Title,
                    RuleOutcome = l.RuleOutcome,
                    MetadataJson = l.MetadataJson,
                    Timestamp = l.Timestamp
                })
                .ToListAsync();


            return View(logs);
        }

        public async Task<IActionResult> Publication(int id)
        {
            var logs = await db.PublicationAuditLogs
                .Where(l => l.PublicationId == id)
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new PublicationAuditLogViewModel
                {
                    PublicationId = l.PublicationId,
                    Title = l.Title,
                    RuleOutcome = l.RuleOutcome,
                    MetadataJson = l.MetadataJson,
                    Timestamp = l.Timestamp
                })
                .ToListAsync();

            ViewBag.PublicationId = id;
            return View("Rules", logs);
        }

    }
}
