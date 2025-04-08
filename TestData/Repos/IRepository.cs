using TestData.Entities;

namespace TestData.Repos
{
    public interface IRepository
    {
        Task<IEnumerable<Warehouse>> AllWarehouses();

        Task AddWarehouse(Warehouse warehouse);
    }
}
