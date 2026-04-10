namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PageSectionsForPane : IPaneDisplay, IDisposable,
    IHandle<StyleSaved>, IHandle<FontSaved>, IHandle<FontRemoved>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private PageSectionsForPaneModel? _model;
    private Guid? _modelPageId;

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? PortalId { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IPanePageService PanePageService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public PageSectionsForPaneModel Model => _model!;
    public Guid? PageId => Path.Id("page") ?? Id;
    public Guid? PreviewPortalId => PortalId ?? Path.Id("portal");
    public Int32 PreviewVersion { get; set; }

    protected override void OnInitialized()
    {
        EventBus.Subscribe(this);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (PageId.Equals(_modelPageId) && _model is not null)
            return;

        if (_model is not null)
        {
            _model.PropertyChanged -= HandleModelChanged;
            _model.Dispose();
        }

        _modelPageId = PageId;
        _model = new(EventBus, ScrollService, PanePageService, PageId);
        _model.PropertyChanged += HandleModelChanged;

        await _model.Initialize();
    }

    public Task Handle(StyleSaved payload) => RefreshPreview(payload.ContentPortalId);
    public Task Handle(FontSaved payload) => RefreshPreview(payload.ContentPortalId);
    public Task Handle(FontRemoved payload) => RefreshPreview(payload.ContentPortalId);

    public void Dispose()
    {
        EventBus.Unsubscribe(this);

        if (_model is not null)
        {
            _model.PropertyChanged -= HandleModelChanged;
            _model.Dispose();
        }
    }

    private Task RefreshPreview(Guid? portalId)
    {
        if (!portalId.Equals(PreviewPortalId))
            return Task.CompletedTask;

        PreviewVersion++;
        return InvokeAsync(StateHasChanged);
    }
}

public class PageSectionsForPaneModel(
    IEventBus eventBus,
    IScrollService scrollService,
    IPanePageService panePageService,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await panePageService.FetchSections(new(new()
        {
            PageId = pageId,
        }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await panePageService.FetchSection(new(new()
        {
            PageId = section.PageId,
            Section = section,
        }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await panePageService.RemoveSection(new(new()
        {
            PageId = section.PageId,
            Section = section,
        }));

    protected override async Task<Response<Section?>> DuplicateSection(Section section) =>
        await panePageService.DuplicateSection(new(new()
        {
            PageId = section.PageId,
            Section = section,
        }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await panePageService.SaveSectionOrder(new(new()
        {
            PageId = sections.FirstOrDefault()?.PageId,
            Sections = sections,
        }));
}