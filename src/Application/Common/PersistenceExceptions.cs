namespace portfolio_todo_rest_api.Application.Common;

public sealed class ConcurrencyConflictException(string message, Exception innerException)
    : Exception(message, innerException);

public sealed class PersistenceConflictException(string message, Exception innerException)
    : Exception(message, innerException);
