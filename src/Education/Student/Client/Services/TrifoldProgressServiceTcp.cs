namespace Crudspa.Education.Student.Client.Services;

public class TrifoldProgressServiceTcp : ITrifoldProgressService, IHandle<TrifoldProgressUpdated>
{
    private readonly IProxyWrappers _proxyWrappers;
    private readonly Lazy<Task> _lazyTrifolds;
    private readonly Object _trifoldsLock = new();
    private readonly SemaphoreSlim _initLock = new(1);
    private List<TrifoldProgress> _trifolds = [];

    public TrifoldProgressServiceTcp(IProxyWrappers proxyWrappers, IEventBus eventBus)
    {
        _proxyWrappers = proxyWrappers;
        _lazyTrifolds = new(FetchTrifoldsFromServer);

        eventBus.Subscribe(this);
    }

    public async Task<Response<IList<TrifoldProgress>>> FetchAll(Request request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyTrifolds.IsValueCreated)
                await _lazyTrifolds.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return new(_trifolds);
    }

    public async Task<TrifoldProgress> Fetch(Request<Trifold> request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyTrifolds.IsValueCreated)
                await _lazyTrifolds.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return _trifolds.FirstOrDefault(x => x.TrifoldId.Equals(request.Value.Id)) ?? new TrifoldProgress
        {
            TrifoldId = request.Value.Id,
            TrifoldCompletedCount = 0,
        };
    }

    public async Task<Response> AddCompleted(Request<TrifoldCompleted> request) =>
        await _proxyWrappers.Send("TrifoldProgressAddCompleted", request);

    public async Task Handle(TrifoldProgressUpdated payload)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyTrifolds.IsValueCreated)
                await _lazyTrifolds.Value;
            else
            {
                lock (_trifoldsLock)
                {
                    _trifolds.RemoveAll(x => x.TrifoldId.Equals(payload.Progress.TrifoldId));
                    _trifolds.Add(payload.Progress);
                }
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task Handle(Reconnected payload)
    {
        await _initLock.WaitAsync();

        try
        {
            await FetchTrifoldsFromServer();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task FetchTrifoldsFromServer()
    {
        var response = await _proxyWrappers.Send<IList<TrifoldProgress>>("TrifoldProgressFetchAll", new());

        if (response.Ok)
            lock (_trifoldsLock)
                _trifolds = new(response.Value);
    }
}