using Ranks.Data_Storage;
using Ranks.Models;
using Ranks.Services;
using System.Collections.Generic;

namespace Ranks.Views;

public partial class HomePage : ContentPage
{
    private readonly PlayerRepository _repository;
    private readonly AppDatabaseService _database;
    private readonly PlayerReposotoryWithDate _reposotoryWithDate;


    public HomePage(PlayerRepository repository, AppDatabaseService database, PlayerReposotoryWithDate reposotoryWithDate)
    {
        InitializeComponent();
        _repository = repository;
        _database = database;
        _reposotoryWithDate = reposotoryWithDate;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void BtnWomensRank_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WomensRanking));
    }

    private async void BtnMensRank_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MensRanking));
    }

    private async void BtnAddTournament_OnClicked(object? sender, EventArgs e)
    {
        var player = new PlayerDB()
        {
            Id = 10000,
            Place = 10000,
            Points = 0,
            PointsWithBonus = 0,
            Name = "Vārds",
            Surname = "Uzvārds",
            Gender = "male",
            OverallPlace = 10000,
            BirthDate = ""
        };
        var p = await _database.GetPlayerAsync(694);
        if (p != null)
        {
            player.Id = 694;
            player.Place = p.Place;
            player.Points = p.Points;
            player.PointsWithBonus = p.PointsWithBonus;
            player.Name = p.Name;
            player.Surname = p.Surname;
            player.Gender = p.Gender;
            player.OverallPlace = p.OverallPlace;
            player.BirthDate = p.BirthDate;
        }
        var tournament = new Tournament()
        {
            Coefficient = "0.5",
            Name = "Jauns turnīrs",
            Date = DateTime.Now,
            TournamentPlayerName = player.Name,
            TournamentPlayerSurname = player.Surname,
            TournamentPlayerPoints = player.Points,
            TournamentPlayerId = player.Id
        };
        await _database.SaveTournamentAsync(tournament);
        Data.TournamentId = tournament.Id;
        await Shell.Current.GoToAsync(nameof(AddTournament));
    }

    private async void BtnEditTournament_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AllTournaments));
    }

    private async void BtnLoadRankings_OnClicked(object? sender, EventArgs e)
    {
        var apiPlayers = await _repository.GetPlayersAsync();
        List<PlayerDB> dbPlayers = await _database.GetPlayersAsync();

        var apiPlayerIds = new HashSet<int>(apiPlayers.Select(p => p.Id));

        var missingPlayers = dbPlayers.Where(dbPlayer => !apiPlayerIds.Contains(dbPlayer.Id)).ToList();

        foreach (var player in missingPlayers)
        {
            await _database.UpdatePlayerAsync(player);
        }

        await _database.UpsertPlayersAsync(apiPlayers);
    }

    private async void BtnGames_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AllGames));
    }

    private async void BtnAllRank_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AllPlayerRanking));
    }

    private async void BtnInactiveRank_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(InactivePlayers));
    }

    private async void BtnGetAllRankings_Clicked(object sender, EventArgs e)
    {
        await SyncPlayersAsync();
    }

    private async Task SyncPlayersAsync()
    {
        int startYear = 2014;
        int currentYear = DateTime.UtcNow.Year;
        int currentMonth = DateTime.UtcNow.Month;
        int endYear = (currentMonth == 1) ? currentYear - 1 : currentYear - 1;

        for (int year = startYear; year <= endYear; year++)
        {
            string dateString = $"{year}-01";
            List<PlayerDB> apiPlayers = await _reposotoryWithDate.GetPlayersAsync(dateString);
            await _database.UpsertPlayersAsync(apiPlayers);
        }
    }
}