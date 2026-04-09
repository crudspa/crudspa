namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PostFindForBlog : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPostService PostService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public PostFindForBlogModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PostService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void AddNew()
    {
        Navigator.GoTo($"{Path}/post-{Guid.NewGuid():D}?state=new");
    }
}

public class PostFindForBlogModel : FindModel<PostSearch, Post>,
    IHandle<PostAdded>, IHandle<PostSaved>, IHandle<PostRemoved>,
    IHandle<SectionAdded>, IHandle<SectionSaved>, IHandle<SectionRemoved>//,
                                                                         //IHandle<CommentAdded>, IHandle<CommentRemoved>,
                                                                         //IHandle<PostReactionAdded>, IHandle<PostReactionRemoved>,
                                                                         //IHandle<PostTagAdded>, IHandle<PostTagRemoved>
{
    private readonly IPostService _postService;
    private readonly Guid? _blogId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public PostFindForBlogModel(IEventBus eventBus, IScrollService scrollService, IPostService postService, Guid? blogId)
        : base(scrollService)
    {
        _postService = postService;
        _blogId = blogId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Published",
            "Revised",
            "Title",
            "Author",
        ];
    }

    public async Task Handle(PostAdded payload) => await Refresh();
    public async Task Handle(PostSaved payload) => await Refresh();
    public async Task Handle(PostRemoved payload) => await Refresh();
    //public async Task Handle(CommentAdded payload) => await Refresh();
    //public async Task Handle(CommentRemoved payload) => await Refresh();
    //public async Task Handle(PostReactionAdded payload) => await Refresh();
    //public async Task Handle(PostReactionRemoved payload) => await Refresh();
    //public async Task Handle(PostTagAdded payload) => await Refresh();
    //public async Task Handle(PostTagRemoved payload) => await Refresh();
    public async Task Handle(SectionAdded payload) => await Refresh();
    public async Task Handle(SectionSaved payload) => await Refresh();
    public async Task Handle(SectionRemoved payload) => await Refresh();

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _blogId;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = true;
        Search.Status.Clear();
        Search.PublishedRange.Type = DateRange.Types.Any;
        Search.RevisedRange.Type = DateRange.Types.Any;

        await WithMany("Initializing...",
            FetchContentStatusNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<PostSearch>(Search);
        var response = await WithWaiting("Searching...", () => _postService.SearchForBlog(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _postService.Remove(new(new()
        {
            Id = id,
            BlogId = _blogId,
        })));
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _postService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }
}