using CdcDataSyncPrototype.BusinessApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CdcDataSyncPrototype.BusinessApp.ViewModels;

namespace CdcDataSyncPrototype.BusinessApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
