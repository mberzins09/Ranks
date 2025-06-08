using Ranks.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranks.Services
{
    public class AppDatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public AppDatabaseService()
        {
            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            InitAsync().Wait();
        }

        private async Task InitAsync()
        {
            await _database.CreateTableAsync<PlayerDB>();
            await _database.CreateTableAsync<Game>();
            await _database.CreateTableAsync<Tournament>();
        }

        // Players
        public async Task<List<PlayerDB>> GetPlayersAsync() =>
            await _database.Table<PlayerDB>().ToListAsync();

        public async Task<PlayerDB> GetPlayerAsync(int id) =>
            await _database.Table<PlayerDB>().Where(x => x.Id == id).FirstOrDefaultAsync();

        public async Task UpdatePlayerAsync(PlayerDB player)
        {
            var existingPlayer = await GetPlayerAsync(player.Id);
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
                var existing = await GetPlayerAsync(player.Id);
                if (existing != null)
                {
                    existing.PointsWithBonus = player.PointsWithBonus;
                    existing.Points = player.Points;
                    existing.Place = player.Place;
                    existing.OverallPlace = player.OverallPlace;

                    await _database.UpdateAsync(existing);
                }
                else
                {
                    await _database.InsertAsync(player);
                }
            }
        }

        // Games
        public async Task<List<Game>> GetGamesAsync() =>
            await _database.Table<Game>().ToListAsync();

        public async Task<Game> GetGameAsync(int id) =>
            await _database.Table<Game>().Where(i => i.Id == id).FirstOrDefaultAsync();

        public async Task<int> SaveGameAsync(Game game) =>
            game.Id != 0 ? await _database.UpdateAsync(game) : await _database.InsertAsync(game);

        public async Task<int> DeleteGameAsync(Game game) =>
            await _database.DeleteAsync(game);

        // Tournaments
        public async Task<List<Tournament>> GetTournamentsAsync() =>
            await _database.Table<Tournament>().ToListAsync();

        public async Task<Tournament> GetTournamentAsync(int id) =>
            await _database.Table<Tournament>().Where(i => i.Id == id).FirstOrDefaultAsync();

        public async Task<int> SaveTournamentAsync(Tournament tournament) =>
            tournament.Id != 0 ? await _database.UpdateAsync(tournament) : await _database.InsertAsync(tournament);

        public async Task<int> DeleteTournamentAsync(Tournament tournament) =>
            await _database.DeleteAsync(tournament);
    }
}
