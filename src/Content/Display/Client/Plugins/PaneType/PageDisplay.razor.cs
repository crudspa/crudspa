using Crudspa.Content.Display.Client.Components;

namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class PageDisplay : IPaneDisplay, IHasPaneId, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? PaneId { get; set; }
    [Parameter] public Guid? PortalId { get; set; }

    [Inject] public IPageRunService PageRunService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;

    public PageDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, PaneId, PageRunService, EventBus);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PageDisplayModel : ScreenModel, IHandle<PageContentChanged>
{
    private readonly Guid? _id;
    private readonly Guid? _paneId;
    private readonly IPageRunService _pageRunService;
    public PageDisplayModel(Guid? id,
        Guid? paneId,
        IPageRunService pageRunService,
        IEventBus eventBus)
    {
        _id = id;
        _paneId = paneId;
        _pageRunService = pageRunService;

        eventBus.Subscribe(this);
    }

    public Page? Page
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Handle(PageContentChanged payload)
    {
        if (payload.Id.Equals(Page?.Id ?? _id))
            await Refresh();
    }

    public async Task Refresh()
    {
        var response = _paneId.HasValue
            ? await WithWaiting("Fetching...", () => _pageRunService.FetchForPane(new(new() { PaneId = _paneId })))
            : await WithWaiting("Fetching...", () => _pageRunService.Fetch(new(new() { Id = _id })));

        if (response.Ok && response.Value is not null)
            SetPage(response.Value);
    }

    private void SetPage(Page page)
    {
        page.Sections ??= [];
        page.Sections = page.Sections.OrderBy(x => x.Ordinal).ToObservable();

        Page = null;
        Page = page;

    }
}