using System.Windows;
using System.Windows.Threading;
using ImHelper.Infrastructure;

namespace ImHelper.Services;

public sealed class WpfSqlExecutionErrorNotifier : ISqlExecutionErrorNotifier
{
    public void ShowError(string message)
    {
        var dispatcher = Application.Current.Dispatcher;
        if (dispatcher.CheckAccess())
        {
            ShowMessage(message);
            return;
        }

        dispatcher.Invoke(() => ShowMessage(message));
    }

    private static void ShowMessage(string message)
    {
        MessageBox.Show(
            message,
            "SQL 처리 오류",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
