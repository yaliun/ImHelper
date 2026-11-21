namespace ImHelper.Infrastructure;

public sealed class SqlFileOperationException(string message, Exception innerException)
    : Exception(message, innerException);
