using TestData.Entities;

namespace TestData.Repos
{
    public interface IRepository
    {
        Task<IEnumerable<Warehouse>> AllWarehouses();
        Task<Warehouse?> GetWarehouse(Guid id);
        Task AddWarehouse(Warehouse warehouse);
    }
}
