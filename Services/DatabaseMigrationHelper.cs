using Ranks.Models;
using SQLite;

namespace Ranks.Services
{
    public static class DatabaseMigrationHelper
    {
        private const string MigrationKey = "HasMigratedToUnifiedDb";

        public static async Task MigrateOldDatabasesAsync()
        {
            // Already migrated? Skip.
            if (Preferences.Get(MigrationKey, false))
                return;

            try
            {
                var unifiedDb = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
                await unifiedDb.CreateTableAsync<PlayerDB>();
                await unifiedDb.CreateTableAsync<Game>();
                await unifiedDb.CreateTableAsync<Tournament>();

                var appDataDir = FileSystem.AppDataDirectory;

                // Paths to old databases
                var oldPlayersPath = Path.Combine(appDataDir, "AllP.db3");
                var oldGamesPath = Path.Combine(appDataDir, "Data3.db3");

                // ----- Migrate Players -----
                if (File.Exists(oldPlayersPath))
                {
                    var oldPlayersDb = new SQLiteAsyncConnection(oldPlayersPath);
                    await oldPlayersDb.CreateTableAsync<PlayerDB>();

                    var players = await oldPlayersDb.Table<PlayerDB>().ToListAsync();
                    foreach (var player in players)
                    {
                        await unifiedDb.InsertOrReplaceAsync(player);
                    }

                    File.Delete(oldPlayersPath);
                }

                // ----- Migrate Games & Tournaments -----
                if (File.Exists(oldGamesPath))
                {
                    var oldGamesDb = new SQLiteAsyncConnection(oldGamesPath);
                    await oldGamesDb.CreateTableAsync<Game>();
                    await oldGamesDb.CreateTableAsync<Tournament>();

                    var games = await oldGamesDb.Table<Game>().ToListAsync();
                    foreach (var game in games)
                    {
                        await unifiedDb.InsertOrReplaceAsync(game);
                    }

                    var tournaments = await oldGamesDb.Table<Tournament>().ToListAsync();
                    foreach (var tournament in tournaments)
                    {
                        await unifiedDb.InsertOrReplaceAsync(tournament);
                    }

                    File.Delete(oldGamesPath);
                }

                // Mark migration as completed
                Preferences.Set(MigrationKey, true);
            }
            catch (Exception ex)
            {
                // Optionally log or handle the error
                Console.WriteLine($"[Migration] Failed: {ex.Message}");
            }
        }
    }
}
