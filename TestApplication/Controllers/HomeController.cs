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

        private static JsonSerializerOptions GetSerializerOptions()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            serializeOptions.Converters.Add(new GeometryJsonConverter());
            return serializeOptions;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await _repository.AllWarehouses();

            

            var jsonString = JsonSerializer.Serialize(result, GetSerializerOptions());

            return Ok(jsonString);
        }

        [HttpGet]
        public IActionResult ValidationError()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse([FromBody] WarehouseDto warehouseDto)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (warehouseDto.Geometry is null)
            {
                return BadRequest(warehouseDto.Geometry);
            }
            var geoJsonReader = new GeoJsonReader();
            var geometry = geoJsonReader.Read<Geometry?>(warehouseDto.Geometry);

            var warehouse = new Warehouse()
            {
                Id = Guid.NewGuid(),
                Name = warehouseDto.Name,
                Director = warehouseDto.Director,
                ActivityType = warehouseDto.ActivityType,
                Address = warehouseDto.Address,
                Geometry = geometry,
            };
                              
            await _repository.AddWarehouse(warehouse);
            return Json(new { success = true });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
