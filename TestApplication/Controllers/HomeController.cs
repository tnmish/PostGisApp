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
using GeometryType = TestData.Enums.GeometryType;

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
        public IActionResult AddWarehouse(GeometryType type, double lat, double lng)
        {
            var newWirehouse = new WarehouseDto()
            {
                Id = Guid.NewGuid(),
            };

            switch (type)
            {
                case GeometryType.Point:
                    newWirehouse.Type = type;
                    newWirehouse.Latitude = lat;
                    newWirehouse.Longitude = lng;
                    break;
            }


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
        public IActionResult ValidationError()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse(WarehouseDto warehouseDto)
        {
            if (ModelState.IsValid)
            {
                if (warehouseDto.Type is null)
                {
                    return BadRequest();
                }

                var geometryFactory = new GeometryFactory();

                var warehouse = new Warehouse()
                {
                    Id = warehouseDto.Id,
                    Name = warehouseDto.Name,
                    Director = warehouseDto.Director,
                    ActivityType = warehouseDto.ActivityType,
                    Address = warehouseDto.Address,
                    Geometry = geometryFactory.CreatePoint(new Coordinate(warehouseDto.Latitude, warehouseDto.Longitude)),
                };
                                
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
