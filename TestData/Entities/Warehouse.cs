using TestData.Enums;

namespace TestData.Entities
{
    public class Warehouse
    {
        public Guid Id { get; init; }
        public string Address { get; init; } = "";
        public ActivityType ActivityType { get; init; }
    }
}