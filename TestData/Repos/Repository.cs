using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using TestData.Entities;

namespace TestData.Repos
{
    public class Repository (TestDbContext dbContext, IConfiguration configuration) : IRepository
    {
        private readonly TestDbContext _dbContext = dbContext;
        private readonly IConfiguration _configuration = configuration;
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

        public async Task<Warehouse?> GetWarehouse(Guid id)
        {
            return await _dbContext
                .Warehouse
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
