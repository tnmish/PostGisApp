using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using Npgsql;
using TestData.Entities;

namespace TestData.Repos
{
    public class RepositoryADO(IConfiguration configuration) : IRepository
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly string connectionString = configuration["Database:ConnectionString"] ?? throw new ArgumentNullException("Database:ConnectionString");

        public async Task AddWarehouse(Warehouse warehouse)
        {            
            // Включаем поддержку PostGIS
            var npgsqlConnBuilder = new NpgsqlConnectionStringBuilder(connectionString);
            var writer = new NetTopologySuite.IO.WKTWriter();

            using var conn = new NpgsqlConnection(npgsqlConnBuilder.ConnectionString);
            conn.Open();

            if (HasInterception(conn, warehouse.Geometry))
            {
                return;
            }

            string point = writer.Write(warehouse.Geometry);

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO test.""Warehouse"" (""Id"", ""Name"", ""Address"", ""ActivityType"", ""Director"", ""Geometry"")
                VALUES (@Id, @Name, @Address, @ActivityType, @Director, ST_GeomFromText(@WKT, 4326));
                ", conn);
            cmd.Parameters.AddWithValue("Id", warehouse.Id);
            cmd.Parameters.AddWithValue("Name", warehouse.Name);
            cmd.Parameters.AddWithValue("Address", warehouse.Address);
            cmd.Parameters.AddWithValue("ActivityType", warehouse.ActivityType is null ? DBNull.Value : warehouse.ActivityType);
            cmd.Parameters.AddWithValue("Director", warehouse.Director);
            cmd.Parameters.AddWithValue("WKT", warehouse.Geometry is null ? DBNull.Value : point);

            await cmd.ExecuteNonQueryAsync();
        }

        private static bool HasInterception(NpgsqlConnection conn, Geometry? newGeometry)
        {
            if (newGeometry == null) return false;

            var wktWriter = new NetTopologySuite.IO.WKTWriter();
            string newWkt = wktWriter.Write(newGeometry);

            using var cmd = new NpgsqlCommand(@"
                SELECT EXISTS (
                    SELECT 1
                    FROM test.""Warehouse""
                    WHERE ST_Intersects(""Geometry"", ST_GeomFromText(@NewWKT, 4326))
                );
            ", conn);
            cmd.Parameters.AddWithValue("NewWKT", newWkt);

            return (bool?)cmd.ExecuteScalar() ?? false;
        }

        public async Task<IEnumerable<Warehouse>> AllWarehouses()
        {
            var npgsqlConnBuilder = new NpgsqlConnectionStringBuilder(connectionString);
            var result = new List<Warehouse>();

            using var conn = new NpgsqlConnection(npgsqlConnBuilder.ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                SELECT ""Id"", ""Name"", ""Address"", ""ActivityType"", ""Director"", ST_AsText(""Geometry"") AS GeometryWKT
                FROM test.""Warehouse"";
                ", conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    Geometry? geometry = null;
                    if (!reader.IsDBNull(5))
                    {
                        string wkt = reader.GetString(5);

                        // Преобразуем WKT обратно в Geometry
                        var wktReader = new NetTopologySuite.IO.WKTReader();
                        geometry = wktReader.Read(wkt);
                    }
                    result.Add(new Warehouse()
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Address = reader.GetString(2),
                        ActivityType = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Director = reader.GetString(4),
                        Geometry = geometry,
                    });
                }
            }

            return result;
        }
    }
}
