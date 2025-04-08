using TestData.Entities;

namespace TestData.Repos
{
    public interface IRepository
    {
        Task<IEnumerable<Warehouse>> AllWarehousesAsync();

        Task AddWarehouseAsync(Warehouse warehouse);
    }
}
