namespace Crudspa.Content.Display.Client.Services;

public class ElementProgressServiceTcp : IElementProgressService, IHandle<ElementProgressUpdated>, IHandle<Reconnected>
{
    private readonly IProxyWrappers _proxyWrappers;
    private readonly Lazy<Task> _lazyElements;
    private readonly Object _elementsLock = new();
    private readonly SemaphoreSlim _initLock = new(1);
    private List<ElementProgress> _elements = [];

    public ElementProgressServiceTcp(IProxyWrappers proxyWrappers, IEventBus eventBus)
    {
        _proxyWrappers = proxyWrappers;
        _lazyElements = new(FetchElementsFromServer);

        eventBus.Subscribe(this);
    }

    public async Task<Response<IList<ElementProgress>>> FetchAll(Request request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyElements.IsValueCreated)
                await _lazyElements.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return new(_elements);
    }

    public async Task<ElementProgress> Fetch(Request<Element> request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyElements.IsValueCreated)
                await _lazyElements.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return _elements.FirstOrDefault(x => x.ElementId.Equals(request.Value.Id)) ?? new ElementProgress
        {
            ElementId = request.Value.Id,
            TimesCompleted = 0,
        };
    }

    public async Task<Response> AddCompleted(Request<ElementCompleted> request) =>
        await _proxyWrappers.Send("ElementProgressAddCompleted", request);

    public async Task<Response> AddLink(Request<ElementLink> request) =>
        await _proxyWrappers.Send("ElementProgressAddLink", request);

    public async Task Handle(ElementProgressUpdated payload)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyElements.IsValueCreated)
                await _lazyElements.Value;
            else
            {
                lock (_elementsLock)
                {
                    _elements.RemoveAll(x => x.ElementId.Equals(payload.Progress.ElementId));
                    _elements.Add(payload.Progress);
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
            await FetchElementsFromServer();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task FetchElementsFromServer()
    {
        var response = await _proxyWrappers.Send<IList<ElementProgress>>("ElementProgressFetchAll", new());

        if (response.Ok)
            lock (_elementsLock)
                _elements = new(response.Value);
    }
}