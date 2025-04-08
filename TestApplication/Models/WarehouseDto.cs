using NetTopologySuite.Geometries;
using TestData.Enums;

namespace TestApplication.Models
{
    public class WarehouseDto
    {
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
        public string? Geometry { get; init; }
    }
}
