using Microsoft.EntityFrameworkCore;
using TestData.Entities;

namespace TestData.Repos
{
    [Obsolete ("Использовать RepositoryADO")]
    public class Repository(TestDbContext dbContext) : IRepository
    {
        private readonly TestDbContext _dbContext = dbContext;

        public async Task AddWarehouseAsync(Warehouse warehouse)
        {
            _dbContext.Warehouse.Add(warehouse);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Warehouse>> AllWarehousesAsync()
        {
            return await _dbContext
                .Warehouse
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
