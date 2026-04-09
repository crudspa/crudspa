namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class ContentPortalSections : IDisposable,
    IHandle<StyleSaved>, IHandle<FontSaved>, IHandle<FontRemoved>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public String? Path { get; set; }
    [Parameter, EditorRequired] public Guid? ContentPortalId { get; set; }
    [Parameter, EditorRequired] public Guid? PageId { get; set; }

    [Inject] public IContentPortalService ContentPortalService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ContentPortalSectionsModel Model { get; set; } = null!;
    public Int32 PreviewVersion { get; set; }

    protected override async Task OnInitializedAsync()
    {
        EventBus.Subscribe(this);

        Model = new(EventBus, ScrollService, ContentPortalService, ContentPortalId, PageId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public Task Handle(StyleSaved payload) => RefreshPreview(payload.ContentPortalId);
    public Task Handle(FontSaved payload) => RefreshPreview(payload.ContentPortalId);
    public Task Handle(FontRemoved payload) => RefreshPreview(payload.ContentPortalId);

    public void Dispose()
    {
        EventBus.Unsubscribe(this);
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private Task RefreshPreview(Guid? portalId)
    {
        if (!portalId.Equals(ContentPortalId))
            return Task.CompletedTask;

        PreviewVersion++;
        return InvokeAsync(StateHasChanged);
    }
}

public class ContentPortalSectionsModel(
    IEventBus eventBus,
    IScrollService scrollService,
    IContentPortalService contentPortalService,
    Guid? contentPortalId,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await contentPortalService.FetchSections(new(new() { ContentPortalId = contentPortalId }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await contentPortalService.FetchSection(new(new()
        {
            ContentPortalId = contentPortalId,
            Section = section,
        }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await contentPortalService.RemoveSection(new(new()
        {
            ContentPortalId = contentPortalId,
            Section = section,
        }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await contentPortalService.SaveSectionOrder(new(new()
        {
            ContentPortalId = contentPortalId,
            Sections = sections,
        }));
}