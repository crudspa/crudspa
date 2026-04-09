namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class PostDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IBlogRunService BlogRunService { get; set; } = null!;

    public PostDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<PostConfig>();

        if (config is not null && config.IdSource == PostConfig.IdSources.SpecificPost && config.PostId.HasSomething())
            Id = config.PostId;

        Model = new(Path, Id, EventBus, Navigator, BlogRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PostDisplayModel : ScreenModel
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IBlogRunService _blogRunService;

    public PostDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IBlogRunService blogRunService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _blogRunService = blogRunService;

        eventBus.Subscribe(this);
    }

    public Post? Post
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _blogRunService.FetchPost(new(new() { Id = _id })));

        if (response.Ok)
            await SetPost(response.Value);
    }

    private Task SetPost(Post post)
    {
        Post = post;

        _navigator.UpdateTitle(_path, Post.Title!);

        return Task.CompletedTask;
    }
}