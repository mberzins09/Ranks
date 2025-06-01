using Microsoft.Extensions.Logging;
using Ranks;
using Ranks.Data_Storage;
using Ranks.Services;
using Ranks.ViewModels;
using Ranks.Views;
using Ranks.Services;

namespace Ranks
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<PlayerService>();
            builder.Services.AddSingleton<PlayerServiceWithDate>();
            builder.Services.AddSingleton<RankingAppsDatabase>();
            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddSingleton<PlayerViewModel>();
            builder.Services.AddSingleton<GameViewModel>();
            builder.Services.AddSingleton<PlayerRepository>();
            builder.Services.AddSingleton<PlayerReposotoryWithDate>();
            builder.Services.AddSingleton<TournamentViewModel>();
            builder.Services.AddSingleton<AllTournamentsViewModel>();
            builder.Services.AddSingleton<AllGamesViewModel>();
            builder.Services.AddSingleton<AddTournamentViewModel>();

            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddTransient<AllPlayerRanking>();
            builder.Services.AddTransient<MensRanking>();
            builder.Services.AddTransient<WomensRanking>();
            builder.Services.AddTransient<InactivePlayers>();
            builder.Services.AddTransient<Tournaments>();
            builder.Services.AddTransient<Games>();
            builder.Services.AddTransient<AllTournaments>();
            builder.Services.AddTransient<AllGames>();
            builder.Services.AddTransient<AddTournament>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
