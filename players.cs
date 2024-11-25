using System.Data;
using System.Numerics;
using Dapper;

namespace Core
{
    public class PlayerService
    {
        private readonly IDbConnection _db;

        public PlayerService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> AddPlayerAsync(string name, string tag, string avatar)
        {
            var sql = "INSERT INTO player (name, tag, avatar) VALUES (@Name, @Tag, @Avatar) RETURNING player_id;";
            return await _db.ExecuteScalarAsync<int>(sql, new { Name = name, Tag = tag, Avatar = avatar });
        }

        public async Task<Player> GetPlayerByIdAsync(int playerId)
        {
            var sql = "SELECT * FROM player WHERE player_id = @PlayerId;";
            return await _db.QuerySingleOrDefaultAsync<Player>(sql, new { PlayerId = playerId });
        }

        public async Task<Player> GetPlayerByTagAsync(string tag)
        {
            var sql = "SELECT * FROM player WHERE tag = @Tag;";
            return await _db.QuerySingleOrDefaultAsync<Player>(sql, new { Tag = tag });
        }

        public async Task<bool> UpdatePlayerAsync(int playerId, string name, string avatar)
        {
            var sql = "UPDATE player SET name = @Name, avatar = @Avatar WHERE player_id = @PlayerId;";
            var rowsAffected = await _db.ExecuteAsync(sql, new { Name = name, Avatar = avatar, PlayerId = playerId });
            return rowsAffected > 0;
        }
    }

}
