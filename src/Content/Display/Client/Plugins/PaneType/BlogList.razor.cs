namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class BlogList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IBlogRunService BlogRunService { get; set; } = null!;

    public BlogListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, EventBus, Navigator, BlogRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class BlogListModel : ScreenModel
{
    private readonly String? _path;
    private readonly INavigator _navigator;
    private readonly IBlogRunService _blogRunService;

    public BlogListModel(String? path, IEventBus eventBus,
        INavigator navigator,
        IBlogRunService blogRunService)
    {
        _path = path;
        _navigator = navigator;
        _blogRunService = blogRunService;

        eventBus.Subscribe(this);
    }

    public ObservableCollection<Blog>? Blogs
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _blogRunService.FetchBlogs(new()));

        if (response.Ok)
            await SetBlogs(response.Value);
    }

    public void GoToBlog(Guid? id)
    {
        _navigator.GoTo($"{_path}/blog-{id:D}");
    }

    public async Task Handle(ContactAchievementAdded payload)
    {
        await Refresh();
    }

    private Task SetBlogs(IList<Blog> blogs)
    {
        Blogs = blogs.ToObservable();
        return Task.CompletedTask;
    }
}