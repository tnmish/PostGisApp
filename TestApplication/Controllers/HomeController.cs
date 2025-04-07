using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TestApplication.Models;
using TestData;
using TestData.Entities;

namespace TestApplication.Controllers
{
    public class HomeController(TestDbContext dbContext, 
        ILogger<HomeController> logger) : Controller
    {
        private readonly TestDbContext _dbContext = dbContext;
        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddWarehouse()
        {
            var newWirehouse = new Warehouse()
            {
                Id = Guid.NewGuid(),
            };

            return PartialView(newWirehouse);
        }

        [HttpGet]
        public IActionResult ValidationError()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse(Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Warehouse.Add(warehouse);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
