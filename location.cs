using System.Data;
using Dapper;

namespace Core
{
    public class LocationService
    {
        private readonly IDbConnection _db;

        public LocationService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> AddLocationAsync(string name)
        {
            var sql = "INSERT INTO location (name) VALUES (@Name) RETURNING location_id;";
            return await _db.ExecuteScalarAsync<int>(sql, new { Name = name });
        }

        public async Task<Location> GetLocationByIdAsync(int locationId)
        {
            var sql = "SELECT * FROM location WHERE location_id = @LocationId;";
            return await _db.QuerySingleOrDefaultAsync<Location>(sql, new { LocationId = locationId });
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            var sql = "SELECT * FROM location;";
            return await _db.QueryAsync<Location>(sql);
        }

        public async Task<bool> UpdateLocationAsync(int locationId, string name)
        {
            var sql = "UPDATE location SET name = @Name WHERE location_id = @LocationId;";
            var rowsAffected = await _db.ExecuteAsync(sql, new { Name = name, LocationId = locationId });
            return rowsAffected > 0;
        }
    }

}
