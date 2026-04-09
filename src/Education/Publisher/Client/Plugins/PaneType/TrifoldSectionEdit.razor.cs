namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class TrifoldSectionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? TrifoldId => Path.Id("trifold");
    private Guid? PageId => Path.Id("page");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IContainerService ContainerService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IItemService ItemService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISectionService SectionService { get; set; } = null!;
    [Inject] public ITrifoldService TrifoldService { get; set; } = null!;

    public TrifoldSectionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, SectionService, ItemService, ContainerService, TrifoldService, TrifoldId, PageId);
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

public class TrifoldSectionEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    ISectionService sectionService,
    IItemService itemService,
    IContainerService containerService,
    ITrifoldService trifoldService,
    Guid? trifoldId,
    Guid? pageId)
    : SectionEditModel(path, id, isNew, pageId, eventBus, navigator, scrollService, sectionService, itemService, containerService)
{
    protected override async Task<Response<Section?>> FetchSection(Guid? id) =>
        await trifoldService.FetchSection(new(new()
        {
            TrifoldId = trifoldId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = new() { Id = id },
        }));

    protected override async Task<Response<Section?>> AddSection(Section section) =>
        await trifoldService.AddSection(new(new()
        {
            TrifoldId = trifoldId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = section,
        }));

    protected override async Task<Response> SaveSection(Section section) =>
        await trifoldService.SaveSection(new(new()
        {
            TrifoldId = trifoldId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = section,
        }));
}