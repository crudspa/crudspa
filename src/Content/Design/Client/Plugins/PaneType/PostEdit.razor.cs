namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PostEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPostService PostService { get; set; } = null!;

    public PostEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var blogId = Path!.Id("blog");

        Model = new(Path, Id, IsNew, blogId, EventBus, Navigator, PostService);
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

public class PostEditModel : EditModel<Post>, IHandle<PostSaved>, IHandle<PostRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _blogId;
    private readonly INavigator _navigator;
    private readonly IPostService _postService;

    public PostEditModel(String? path, Guid? id, Boolean isNew, Guid? blogId,
        IEventBus eventBus,
        INavigator navigator,
        IPostService postService) : base(isNew)
    {
        _path = path;
        _id = id;
        _blogId = blogId;
        _navigator = navigator;
        _postService = postService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PostSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(PostRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> PageTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchPageTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetPost(new()
            {
                BlogId = _blogId,
                Title = "New Post",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                Author = String.Empty,
                Page = new()
                {
                    TypeId = PageTypeNames.MinBy(x => x.Ordinal)?.Id,
                },
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _postService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetPost(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _postService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/post-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _postService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _postService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchPageTypeNames()
    {
        var response = await WithAlerts(() => _postService.FetchPageTypeNames(new()), false);
        if (response.Ok) PageTypeNames = response.Value.ToList();
    }

    private void SetPost(Post post)
    {
        Entity = post;
        _navigator.UpdateTitle(_path, Entity.Title!);
    }
}