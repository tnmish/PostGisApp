using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Diagnostics;
using System.Text.Json;
using TestApplication.Models;
using TestData;
using TestData.Entities;
using TestData.Repos;
using TestApplication.Converters;

namespace TestApplication.Controllers
{


    public class HomeController(TestDbContext dbContext, 
        IRepository repository,
        ILogger<HomeController> logger) : Controller
    {
        private readonly TestDbContext _dbContext = dbContext;
        private readonly IRepository _repository = repository;
        private readonly ILogger<HomeController> _logger = logger;
        private readonly GeoJsonWriter geoJsonWriter = new();

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
        public async Task<IActionResult> GetList()
        {
            var result = await _repository.AllWarehouses();

            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            serializeOptions.Converters.Add(new GeometryJsonConverter());

            var jsonString = JsonSerializer.Serialize(result, serializeOptions);

            return Ok(jsonString);
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouse(Guid id)
        {
            var result = await _repository.GetWarehouse(id);
            if (result == null)
            {
                return NotFound();
            }

            return Json(result);
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
                if (warehouse.Geometry is null)
                {
                    warehouse.Geometry = new Point(56.8516, 60.6122);
                }

                var warehouses = await _repository.AllWarehouses();
                if (GeometryIntersectionChecker.IntercectsAnyWithIndex(warehouse.Geometry, warehouses.Select(x => x.Geometry)))
                {
                    return BadRequest();
                }

                await _repository.AddWarehouse(warehouse);
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
