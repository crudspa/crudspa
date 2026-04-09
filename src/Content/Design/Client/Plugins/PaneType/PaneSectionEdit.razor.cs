namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PaneSectionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? PageId => Path.Id("page");
    private Guid? SegmentId => Path.Id("segment");
    private Guid? PaneId => ConfigJson.FromJson<PaneSectionConfig>()?.PaneId;

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IContainerService ContainerService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IItemService ItemService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPanePageService PanePageService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISectionService SectionService { get; set; } = null!;

    public PaneSectionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, SectionService, ItemService, ContainerService, PanePageService, PageId, PaneId, SegmentId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class PaneSectionEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    ISectionService sectionService,
    IItemService itemService,
    IContainerService containerService,
    IPanePageService panePageService,
    Guid? pageId,
    Guid? paneId,
    Guid? segmentId)
    : SectionEditModel(path, id, isNew, pageId, eventBus, navigator, scrollService, sectionService, itemService, containerService)
{
    protected override async Task<Response<Section?>> FetchSection(Guid? id) =>
        await panePageService.FetchSection(new(new()
        {
            SegmentId = segmentId,
            PaneId = paneId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = new() { Id = id },
        }));

    protected override async Task<Response<Section?>> AddSection(Section section) =>
        await panePageService.AddSection(new(new()
        {
            SegmentId = segmentId,
            PaneId = paneId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = section,
        }));

    protected override async Task<Response> SaveSection(Section section) =>
        await panePageService.SaveSection(new(new()
        {
            SegmentId = segmentId,
            PaneId = paneId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = section,
        }));
}