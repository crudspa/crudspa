using Microsoft.AspNetCore.SignalR.Client;

namespace Crudspa.Framework.Core.Client.Services;

public class ProxyWrappersCore : IProxyWrappers
{
    private static readonly String CategoryName = typeof(ProxyWrappersCore).FullName ?? nameof(ProxyWrappersCore);
    private readonly IEventBus _eventBus;
    private readonly ICacheService _cacheService;

    public HubConnection Connection { get; set; }
    public Guid? SessionId { get; private set; }

    public ProxyWrappersCore(IUriProvider uriProvider, IEventBus eventBus, ICacheService cacheService)
    {
        _eventBus = eventBus;
        _cacheService = cacheService;

        Connection = new HubConnectionBuilder()
            .AddJsonProtocol()
            .WithUrl(uriProvider.Root() + "/hub")
            .WithAutomaticReconnect([TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(25), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(45), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5)])
            .Build();

        Connection.On<Notice>("NoticePosted", notice =>
        {
            if (notice.Payload.HasSomething() && notice.Type.HasSomething())
            {
                var eventObject = notice.Payload.FromJson(notice.Type!);
                if (eventObject is not null) eventBus.Publish(eventObject);
            }
        });

        Connection.Reconnected += HandleReconnected;
    }

    private async Task HandleReconnected(String? arg)
    {
        if (await EnsureConnection())
            await Connection.InvokeAsync("Subscribe", new Request { SessionId = SessionId });

        await _eventBus.Publish(new Reconnected());
    }

    public async Task SetSessionId(Guid? sessionId)
    {
        SessionId = sessionId;

        if (await EnsureConnection())
            await Connection.InvokeAsync("Subscribe", new Request { SessionId = SessionId });
    }

    public Task Log(ClientLogEntry entry)
        => SendLog(entry);

    public Task<Response> Send(String methodName, Request request)
        => Invoke<Response>(methodName, request, methodName != "Log");

    public Task<Response<T>> Send<T>(String methodName, Request request) where T : class?
        => Invoke<Response<T>>(methodName, request);

    private async Task SendLog(ClientLogEntry entry)
    {
        try
        {
            var response = await Invoke<Response>("Log", new Request<ClientLogEntry>(entry), false);

            if (!response.Ok)
                await Console.Error.WriteLineAsync($"{CategoryName} could not relay a client log entry.{Environment.NewLine}{response.ErrorMessages}");
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"{CategoryName} could not relay a client log entry.{Environment.NewLine}{ex}");
        }
    }

    private async Task<TResponse> Invoke<TResponse>(String methodName, Request request, Boolean logErrors = true)
        where TResponse : Response, new()
    {
        request.SessionId = SessionId;

        var response = new TResponse();

        if (!await EnsureConnection())
        {
            response.AddError(Constants.ErrorMessages.ServiceCallFailed);
            return response;
        }

        try
        {
            return await Connection.InvokeAsync<TResponse>(methodName, request);
        }
        catch (Exception ex) when (IsTransientError(ex)) { }
        catch (Exception ex)
        {
            if (logErrors)
                _ = SendLog(ClientLoggerCore.CreateEntry(CategoryName, LogLevel.Error, "Unexpected proxy error.", ex, ("MethodName", methodName), ("SessionId", SessionId?.ToString("D"))));
        }

        response.AddError(Constants.ErrorMessages.ServiceCallFailed);
        return response;
    }

    public Task<Response<T>> SendAndCache<T>(String methodName, Request request)
        where T : class?
    {
        if (request.IsGeneric)
        {
            _ = SendLog(ClientLoggerCore.CreateEntry(CategoryName, LogLevel.Warning, "SendAndCache called with generic request. Bypassing client-side cache.", ("MethodName", methodName)));
            return Send<T>(methodName, request);
        }

        var cacheKey = $"{typeof(T).FullName}-{methodName}";
        return _cacheService.GetOrAdd(cacheKey, () => Send<T>(methodName, request));
    }

    private async Task<Boolean> EnsureConnection()
    {
        var attempts = 0;

        while (attempts < 10)
        {
            if (Connection.State == HubConnectionState.Connected)
                return true;

            try
            {
                if (Connection.State == HubConnectionState.Disconnected)
                {
                    await Connection.StartAsync(CancellationToken.None);
                    continue;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempts)));
            }
            finally
            {
                attempts++;
            }
        }

        return Connection.State == HubConnectionState.Connected;
    }

    private static Boolean IsTransientError(Exception ex)
    {
        if (IsTransient(ex))
            return true;

        if (ex is AggregateException aggregateException)
            return aggregateException.Flatten().InnerExceptions.Any(IsTransientError);

        return ex.InnerException is not null && IsTransientError(ex.InnerException);
    }

    private static Boolean IsTransient(Exception ex)
    {
        return ex is TimeoutException
            || ex is TaskCanceledException
            || ex is OperationCanceledException
            || ex is System.Net.WebSockets.WebSocketException
            || (ex is InvalidOperationException invalidOperation && invalidOperation.Message.Has("connection is not active"));
    }
}