namespace Crudspa.Education.Student.Client.Services;

public class ChapterProgressServiceTcp : IChapterProgressService, IHandle<ChapterProgressUpdated>
{
    private readonly IProxyWrappers _proxyWrappers;
    private readonly Lazy<Task> _lazyChapters;
    private readonly Object _chaptersLock = new();
    private readonly SemaphoreSlim _initLock = new(1);
    private List<ChapterProgress> _chapters = [];

    public ChapterProgressServiceTcp(IProxyWrappers proxyWrappers, IEventBus eventBus)
    {
        _proxyWrappers = proxyWrappers;
        _lazyChapters = new(FetchChaptersFromServer);

        eventBus.Subscribe(this);
    }

    public async Task<Response<IList<ChapterProgress>>> FetchAll(Request request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyChapters.IsValueCreated)
                await _lazyChapters.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return new(_chapters);
    }

    public async Task<ChapterProgress> Fetch(Request<Chapter> request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyChapters.IsValueCreated)
                await _lazyChapters.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return _chapters.FirstOrDefault(x => x.ChapterId.Equals(request.Value.Id)) ?? new ChapterProgress
        {
            ChapterId = request.Value.Id,
            TimesCompleted = 0,
        };
    }

    public async Task<Response> AddCompleted(Request<ChapterCompleted> request) =>
        await _proxyWrappers.Send("ChapterProgressAddCompleted", request);

    public async Task Handle(ChapterProgressUpdated payload)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyChapters.IsValueCreated)
                await _lazyChapters.Value;
            else
            {
                lock (_chaptersLock)
                {
                    _chapters.RemoveAll(x => x.ChapterId.Equals(payload.Progress.ChapterId));
                    _chapters.Add(payload.Progress);
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
            await FetchChaptersFromServer();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task FetchChaptersFromServer()
    {
        var response = await _proxyWrappers.Send<IList<ChapterProgress>>("ChapterProgressFetchAll", new());

        if (response.Ok)
            lock (_chaptersLock)
                _chapters = new(response.Value);
    }
}