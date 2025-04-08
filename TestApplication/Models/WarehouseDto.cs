using NetTopologySuite.Geometries;
using TestData.Enums;

namespace TestApplication.Models
{
    public class WarehouseDto
    {
        public Guid Id { get; init; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; init; } = "";

        /// <summary>
        /// Директор
        /// </summary>
        public string Director { get; init; } = "";

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; init; } = "";

        /// <summary>
        /// Вид деятельности
        /// </summary>
        public string? ActivityType { get; init; }

        /// <summary>
        /// Координаты
        /// </summary>
        public GeometryType? Type { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
