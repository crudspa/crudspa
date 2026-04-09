namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PostSectionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? PostId => Path.Id("post");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IContainerService ContainerService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IItemService ItemService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPostService PostService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISectionService SectionService { get; set; } = null!;

    public PostSectionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, SectionService, ItemService, ContainerService, PostService, PostId);
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

public class PostSectionEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    ISectionService sectionService,
    IItemService itemService,
    IContainerService containerService,
    IPostService postService,
    Guid? postId)
    : SectionEditModel(path, id, isNew, null, eventBus, navigator, scrollService, sectionService, itemService, containerService)
{
    protected override async Task<Response<Section?>> FetchSection(Guid? id) =>
        await postService.FetchSection(new(new()
        {
            PostId = postId,
            Section = new() { Id = id },
        }));

    protected override async Task<Response<Section?>> AddSection(Section section) =>
        await postService.AddSection(new(new()
        {
            PostId = postId,
            Section = section,
        }));

    protected override async Task<Response> SaveSection(Section section) =>
        await postService.SaveSection(new(new()
        {
            PostId = postId,
            Section = section,
        }));
}