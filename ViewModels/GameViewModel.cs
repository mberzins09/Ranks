using System.Collections.ObjectModel;
using Ranks.Models;
using Ranks.Services;

namespace Ranks.ViewModels
{
    public class GameViewModel(AppDatabaseService database) : BaseViewModel
    {
        private readonly AppDatabaseService _database = database;
        public ObservableCollection<PlayerDB>? Players { get; set; }
        public List<PlayerDB> PlayerList { get; set; } = new List<PlayerDB>();
        public bool IsOpponentForeign { get; set; }
        public required Game Game { get; set; }

        public async Task LoadDataAsync()
        {
            Game = await _database.GetGameAsync(Data.GameId);
            var tournament = await _database.GetTournamentAsync(Game.TournamentId);
            var players = await _database.GetPlayersAsync();
            PlayerList = players
                .Where(x =>
                    x.Id != tournament.TournamentPlayerId)
                .OrderByDescending(x => x.PointsWithBonus)
                .ToList();

            Players = new ObservableCollection<PlayerDB>(PlayerList);

            OnPropertyChanged(nameof(Players));
        }

        public async Task SaveGameAsync()
        {
            await _database.SaveGameAsync(Game);
        }

        public async Task<List<PlayerDB>> GetPlayers()
        {
            var tournament = await _database.GetTournamentAsync(Game.TournamentId);
            var players = await _database.GetPlayersAsync();
            var list = players
                .Where(x =>
                    x.Id != tournament.TournamentPlayerId)
                .OrderByDescending(x => x.PointsWithBonus)
                .ToList();

            return list;
        }
        public List<PlayerDB> SearchPlayers(List<PlayerDB> players, string filterText)
        {
            var searchedPlayers = players
                .Where(x => (!string.IsNullOrWhiteSpace(x.Name) &&
                             x.Name.StartsWith(filterText, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(x.Surname) &&
                             x.Surname.StartsWith(filterText, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(x.Place.ToString()) &&
                             x.Place.ToString().StartsWith(filterText, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return searchedPlayers;
        }
    }
}