using Microsoft.EntityFrameworkCore;
using TestData.Entities;

namespace TestData
{
    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<Warehouse> WarehouseSet { get; set; }
    }
}
