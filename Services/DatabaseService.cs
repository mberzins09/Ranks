using Ranks.Models;
using SQLite;

namespace Ranks.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AllP.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<PlayerDB>().Wait();
        }

        public async Task<List<PlayerDB>> GetPlayersAsync()
        {
            return await _database.Table<PlayerDB>().ToListAsync();
        }

        public async Task<PlayerDB> GetPlayerAsync(int id)
        {
            var player = await _database.Table<PlayerDB>().Where(x => x.Id == id).FirstOrDefaultAsync();

            return player;
        }

        public async Task UpdatePlayerAsync(PlayerDB player)
        {
            var existingPlayer = await _database.Table<PlayerDB>()
                    .Where(p => p.Id == player.Id)
                    .FirstOrDefaultAsync();
            if (existingPlayer != null)
            {
                existingPlayer.Place = 6000;
                existingPlayer.OverallPlace = 6000;
                await _database.UpdateAsync(existingPlayer);
            }
        }

        public async Task UpsertPlayersAsync(List<PlayerDB> players)
        {
            foreach (var player in players)
            {
                var existingPlayer = await _database.Table<PlayerDB>()
                    .Where(p => p.Id == player.Id)
                    .FirstOrDefaultAsync();

                if (existingPlayer != null)
                {
                    existingPlayer.PointsWithBonus = player.PointsWithBonus;
                    existingPlayer.Points = player.Points;
                    existingPlayer.Place = player.Place;
                    existingPlayer.OverallPlace = player.OverallPlace;

                    await _database.UpdateAsync(existingPlayer);
                }
                else
                {
                    await _database.InsertAsync(player);
                }
            }
        }
    }
}