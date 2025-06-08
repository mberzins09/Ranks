using System.Collections.ObjectModel;
using Ranks.Models;
using Ranks.Services;
using Game = Ranks.Models.Game;

namespace Ranks.ViewModels
{
    public class TournamentViewModel(AppDatabaseService database) : BaseViewModel
    {
        private readonly AppDatabaseService _database = database;
        public ObservableCollection<Game> Games { get; set; } = new();
        public required Tournament Tournament { get; set; }

        public async Task SaveTournamentAsync()
        {
            await _database.SaveTournamentAsync(Tournament);
        }

        public async Task LoadGamesAsync()
        {
            Tournament = await _database.GetTournamentAsync(Data.TournamentId);
            var list = await _database.GetGamesAsync();
            var tgames = list.Where(x => x.TournamentId == Data.TournamentId).ToList();
            Games = new ObservableCollection<Game>(tgames);
            Tournament.PointsDifference = list
                .Where(x => x.TournamentId == Data.TournamentId)
                .Sum(x => x.RatingDifference);

            OnPropertyChanged();
        }
    }
}