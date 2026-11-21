using System.Data.Common;
using System.IO;
using System.Text.Json;
using System.Windows;
using ImHelper.Infrastructure;
using ImHelper.Services;

namespace ImHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ISqlFileExecutor SqlExecutor { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                var baseDirectory = AppContext.BaseDirectory;
                var settings = new DatabaseSettingsLoader().Load(baseDirectory);
                var databaseProvider = new DatabaseProviderFactory().Create(settings, baseDirectory);
                databaseProvider.Initialize();
                SqlExecutor = new SqlFileCommandExecutor(
                    databaseProvider,
                    new WpfSqlExecutionErrorNotifier(),
                    baseDirectory);
            }
            catch (DbException exception)
            {
                ShowDatabaseStartupError(exception);
                return;
            }
            catch (IOException exception)
            {
                ShowDatabaseStartupError(exception);
                return;
            }
            catch (UnauthorizedAccessException exception)
            {
                ShowDatabaseStartupError(exception);
                return;
            }
            catch (JsonException exception)
            {
                ShowDatabaseStartupError(exception);
                return;
            }
            catch (InvalidOperationException exception)
            {
                ShowDatabaseStartupError(exception);
                return;
            }
            catch (NotSupportedException exception)
            {
                ShowDatabaseStartupError(exception);
                return;
            }

            base.OnStartup(e);

            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
        }

        private static void ShowDatabaseStartupError(Exception exception)
        {
            MessageBox.Show(
                $"ImHelper 데이터베이스를 초기화하지 못했습니다.{Environment.NewLine}{exception.Message}",
                "데이터베이스 초기화 오류",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Current.Shutdown(-1);
        }
    }

}
