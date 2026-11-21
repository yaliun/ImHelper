namespace ImHelper.Infrastructure;

public interface ISqlExecutionErrorNotifier
{
    void ShowError(string message);
}
