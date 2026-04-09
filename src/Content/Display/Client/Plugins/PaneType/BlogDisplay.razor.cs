namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class BlogDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IBlogRunService BlogRunService { get; set; } = null!;

    public BlogDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<BlogConfig>();

        if (config is not null && config.IdSource == BlogConfig.IdSources.SpecificBlog && config.BlogId.HasSomething())
            Id = config.BlogId;

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

public class BlogDisplayModel : ScreenModel
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IBlogRunService _blogRunService;

    public BlogDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IBlogRunService blogRunService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _blogRunService = blogRunService;

        eventBus.Subscribe(this);
    }

    public Blog? Blog
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _blogRunService.FetchBlog(new(new() { Id = _id })));

        if (response.Ok)
            await SetBlog(response.Value);
    }

    public async Task Handle(ContactAchievementAdded payload)
    {
        await Refresh();
    }

    public void GoToPost(Guid? id)
    {
        _navigator.GoTo($"{_path}/post-{id:D}");
    }

    private Task SetBlog(Blog blog)
    {
        Blog = blog;
        _navigator.UpdateTitle(_path, Blog.Title!);
        return Task.CompletedTask;
    }
}