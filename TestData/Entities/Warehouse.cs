using NetTopologySuite.Geometries;
using TestData.Enums;

namespace TestData.Entities
{
    public class Warehouse
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
        public Polygon? Сoordinates { get; init; }

        public Geometry? Geometry { get; set; }
    }
}