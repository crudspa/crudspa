namespace Crudspa.Content.Design.Client.Components;

public partial class PageListForBinder : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String Path { get; set; } = null!;
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Func<Task<Response<Page?>>> AddPage { get; set; } = null!;
    [Parameter] public Func<Guid?, Task<Response<IList<Page>>>> FetchPages { get; set; } = null!;
    [Parameter] public Func<Guid?, Task<Response<Page?>>> FetchPage { get; set; } = null!;
    [Parameter] public Func<Guid?, Task<Response>> RemovePage { get; set; } = null!;
    [Parameter] public Func<IList<Page>, Task<Response>> SavePageOrder { get; set; } = null!;

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public PageListForBinderModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, FetchPages, FetchPage, RemovePage, SavePageOrder, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public async Task HandleAddRequested()
    {
        var response = await Model.WithWaiting("Adding...", AddPage);

        if (response.Ok)
            Navigator.GoTo($"{Path}/page-{response.Value.Id:D}");
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PageListForBinderModel : ListOrderablesModel<PageModel>,
    IHandle<PageAdded>, IHandle<PageSaved>, IHandle<PageRemoved>,
    IHandle<SectionAdded>, IHandle<SectionRemoved>
{
    private readonly Func<Guid?, Task<Response<IList<Page>>>> _fetchPages;
    private readonly Func<Guid?, Task<Response<Page?>>> _fetchPage;
    private readonly Func<Guid?, Task<Response>> _removePage;
    private readonly Func<IList<Page>, Task<Response>> _savePageOrder;
    private readonly Guid? _binderId;

    public PageListForBinderModel(
        IEventBus eventBus,
        IScrollService scrollService,
        Func<Guid?, Task<Response<IList<Page>>>> fetchPages,
        Func<Guid?, Task<Response<Page?>>> fetchPage,
        Func<Guid?, Task<Response>> removePage,
        Func<IList<Page>, Task<Response>> savePageOrder,
        Guid? binderId)
        : base(scrollService)
    {
        _fetchPages = fetchPages;
        _fetchPage = fetchPage;
        _removePage = removePage;
        _savePageOrder = savePageOrder;
        _binderId = binderId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PageAdded payload) => await Replace(payload.Id, payload.BinderId);
    public async Task Handle(PageSaved payload) => await Replace(payload.Id, payload.BinderId);
    public async Task Handle(PageRemoved payload) => await Rid(payload.Id, payload.BinderId);

    public async Task Handle(SectionAdded payload)
    {
        if (payload.PageId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.PageId)))
            await Replace(payload.PageId);
    }

    public async Task Handle(SectionRemoved payload)
    {
        if (payload.PageId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.PageId)))
            await Replace(payload.PageId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var response = await WithWaiting("Fetching...", () => _fetchPages(_binderId), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new PageModel(x)).ToList());
    }

    public override async Task<Response<PageModel?>> Fetch(Guid? id)
    {
        var response = await _fetchPage(id);

        if (!response.Ok)
            return new() { Errors = response.Errors };

        return new(new PageModel(response.Value));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _removePage(id);
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_binderId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Page).ToList();
        return await WithWaiting("Saving...", () => _savePageOrder(orderables));
    }
}

public class PageModel : Observable, IDisposable, INamed, IOrderable
{
    public String? Name => Page.Title;

    private Page _page;

    public PageModel(Page page)
    {
        _page = page;
        _page.PropertyChanged += HandlePageChanged;
    }

    public void Dispose()
    {
        _page.PropertyChanged -= HandlePageChanged;
    }

    private void HandlePageChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(Page));
    }

    public Guid? Id
    {
        get => _page.Id;
        set => _page.Id = value;
    }

    public Int32? Ordinal
    {
        get => _page.Ordinal;
        set => _page.Ordinal = value;
    }

    public Page Page
    {
        get => _page;
        set => SetProperty(ref _page, value);
    }
}