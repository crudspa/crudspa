namespace Crudspa.Framework.Core.Client.Services;

public class ClientLoggerProviderCore(IProxyWrappers proxyWrappers) : ILoggerProvider, ISupportExternalScope
{
    private IExternalScopeProvider _scopes = new LoggerExternalScopeProvider();

    public ILogger CreateLogger(String categoryName) => new ClientLoggerCore(categoryName, proxyWrappers, this);
    public void Dispose() { }
    public void SetScopeProvider(IExternalScopeProvider scopes) => _scopes = scopes;

    internal IDisposable Push<TState>(TState state) where TState : notnull => _scopes.Push(state);
    internal void ForEachScope<TState>(Action<Object?, TState> callback, TState state) => _scopes.ForEachScope(callback, state);
}

public class ClientLoggerCore(String categoryName, IProxyWrappers proxyWrappers, ClientLoggerProviderCore provider) : ILogger
{
    public Boolean IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => provider.Push(state);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, String> formatter)
    {
        var message = formatter(state, exception);
        if (!message.HasSomething() && exception is not null)
            message = exception.Message;

        Observe(proxyWrappers.Log(new()
        {
            CategoryName = categoryName,
            LogLevel = (Int32)logLevel,
            EventId = eventId.Id,
            EventName = eventId.Name,
            Message = message,
            Exception = exception?.ToString(),
            Data = Capture(state),
        }));
    }

    internal static ClientLogEntry CreateEntry(String categoryName, LogLevel logLevel, String? message, params (String Name, Object? Value)[] data)
        => CreateEntry(categoryName, logLevel, message, null, data);

    internal static ClientLogEntry CreateEntry(String categoryName, LogLevel logLevel, String? message, Exception? exception, params (String Name, Object? Value)[] data)
    {
        Dictionary<String, Object?> values = new();

        foreach (var value in data)
        {
            if (!value.Name.HasSomething())
                continue;

            if (value.Value is String text && !text.HasSomething())
                continue;

            if (value.Value is not null)
                values[value.Name] = value.Value;
        }

        return new()
        {
            CategoryName = categoryName,
            LogLevel = (Int32)logLevel,
            Message = message,
            Exception = exception?.ToString(),
            Data = values,
        };
    }

    private Dictionary<String, Object?> Capture<TState>(TState state)
    {
        Dictionary<String, Object?> data = new();

        provider.ForEachScope((scope, values) => Add(values, scope), data);
        Add(data, state);
        return data;

        static void Add(Dictionary<String, Object?> data, Object? value)
        {
            if (value is not IEnumerable<KeyValuePair<String, Object?>> values)
                return;

            foreach (var item in values)
                if (item.Key.HasSomething())
                    data[item.Key] = item.Value;
        }
    }

    private static void Observe(Task task)
    {
        if (task.IsCompletedSuccessfully)
            return;

        _ = task.ContinueWith(x => Console.Error.WriteLine($"{nameof(ClientLoggerCore)} could not post a client log entry.{Environment.NewLine}{x.Exception}"),
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);
    }
}