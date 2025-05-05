using CdcDataSyncPrototype.BusinessApp.Data;
using CdcDataSyncPrototype.BusinessApp.Models;
using CdcDataSyncPrototype.BusinessApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CdcDataSyncPrototype.BusinessApp.Controllers
{
    public class PublicationsController(AppDbContext db) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var publications = await db.Publications
                .AsNoTracking()
                .Select(p => new PublicationViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Type = p.Type,
                    InternalOnly = p.InternalOnly,
                    PublishStartDate = p.PublishStartDate,
                    PublishEndDate = p.PublishEndDate,
                    LastModified = p.LastModified
                })
                .ToListAsync();

            return View(publications);
        }

        public IActionResult Create()
        {
            return View(new PublicationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PublicationViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var entity = new Publication
            {
                Title = vm.Title,
                Type = vm.Type,
                InternalOnly = vm.InternalOnly,
                PublishStartDate = vm.PublishStartDate,
                PublishEndDate = vm.PublishEndDate,
                LastModified = DateTime.UtcNow
            };

            db.Publications.Add(entity);
            await db.SaveChangesAsync();

            TempData["Message"] = "Publication created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var publication = await db.Publications.FindAsync(id);
            if (publication == null) return NotFound();

            var vm = new PublicationViewModel
            {
                Id = publication.Id,
                Title = publication.Title,
                Type = publication.Type,
                InternalOnly = publication.InternalOnly,
                PublishStartDate = publication.PublishStartDate,
                PublishEndDate = publication.PublishEndDate,
                LastModified = publication.LastModified
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PublicationViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var entity = await db.Publications.FindAsync(id);
            if (entity == null) return NotFound();

            entity.Title = vm.Title;
            entity.Type = vm.Type;
            entity.InternalOnly = vm.InternalOnly;
            entity.PublishStartDate = vm.PublishStartDate;
            entity.PublishEndDate = vm.PublishEndDate;
            entity.LastModified = DateTime.UtcNow;

            await db.SaveChangesAsync();
            TempData["Message"] = "Publication updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Republish(int id)
        {
            var pub = await db.Publications.FindAsync(id);
            if (pub == null)
            {
                return NotFound();
            }

            pub.LastModified = DateTime.UtcNow;
            await db.SaveChangesAsync();

            TempData["Message"] = $"Publication {id} republished.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pub = await db.Publications.FindAsync(id);
            if (pub == null) return NotFound();

            var vm = new PublicationViewModel
            {
                Id = pub.Id,
                Title = pub.Title,
                Type = pub.Type,
                InternalOnly = pub.InternalOnly,
                PublishStartDate = pub.PublishStartDate
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pub = await db.Publications.FindAsync(id);
            if (pub == null) return NotFound();

            db.Publications.Remove(pub);
            await db.SaveChangesAsync();

            TempData["Message"] = "Publication deleted.";
            return RedirectToAction(nameof(Index));
        }

    }
}
