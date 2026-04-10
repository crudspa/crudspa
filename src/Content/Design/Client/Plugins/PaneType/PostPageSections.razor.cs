namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PostPageSections : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public String? Path { get; set; }
    [Parameter, EditorRequired] public Guid? PostId { get; set; }
    [Parameter, EditorRequired] public Guid? PageId { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IPostService PostService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public PostPageSectionsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PostService, PostId, PageId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PostPageSectionsModel(
    IEventBus eventBus,
    IScrollService scrollService,
    IPostService postService,
    Guid? postId,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await postService.FetchSections(new(new() { PostId = postId }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await postService.FetchSection(new(new()
        {
            PostId = postId,
            Section = section,
        }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await postService.RemoveSection(new(new()
        {
            PostId = postId,
            Section = section,
        }));

    protected override async Task<Response<Section?>> DuplicateSection(Section section) =>
        await postService.DuplicateSection(new(new()
        {
            PostId = postId,
            Section = section,
        }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await postService.SaveSectionOrder(new(new()
        {
            PostId = postId,
            Sections = sections,
        }));
}