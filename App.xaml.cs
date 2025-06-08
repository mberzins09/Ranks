using Ranks.Services;

namespace Ranks
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Task.Run(async () => await DatabaseMigrationHelper.MigrateOldDatabasesAsync()).Wait();

            MainPage = new AppShell();
        }
    }
}
