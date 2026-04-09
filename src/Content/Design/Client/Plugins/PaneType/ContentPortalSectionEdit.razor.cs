namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class ContentPortalSectionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? ContentPortalId => Path.Id("portal");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IContainerService ContainerService { get; set; } = null!;
    [Inject] public IContentPortalService ContentPortalService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IItemService ItemService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISectionService SectionService { get; set; } = null!;

    public ContentPortalSectionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, SectionService, ItemService, ContainerService, ContentPortalService, ContentPortalId);
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

public class ContentPortalSectionEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    ISectionService sectionService,
    IItemService itemService,
    IContainerService containerService,
    IContentPortalService contentPortalService,
    Guid? contentPortalId)
    : SectionEditModel(path, id, isNew, null, eventBus, navigator, scrollService, sectionService, itemService, containerService)
{
    protected override async Task<Response<Section?>> FetchSection(Guid? id) =>
        await contentPortalService.FetchSection(new(new()
        {
            ContentPortalId = contentPortalId,
            Section = new() { Id = id },
        }));

    protected override async Task<Response<Section?>> AddSection(Section section) =>
        await contentPortalService.AddSection(new(new()
        {
            ContentPortalId = contentPortalId,
            Section = section,
        }));

    protected override async Task<Response> SaveSection(Section section) =>
        await contentPortalService.SaveSection(new(new()
        {
            ContentPortalId = contentPortalId,
            Section = section,
        }));
}