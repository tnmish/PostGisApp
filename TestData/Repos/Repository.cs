using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using TestData.Entities;

namespace TestData.Repos
{
    public class Repository(TestDbContext dbContext) : IRepository
    {
        private readonly TestDbContext _dbContext = dbContext;

        public async Task AddWarehouse(Warehouse warehouse)
        {
            _dbContext.Warehouse.Add(warehouse);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Warehouse>> AllWarehouses()
        {
            return await _dbContext
                .Warehouse
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
