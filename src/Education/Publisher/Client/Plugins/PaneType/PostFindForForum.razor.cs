using IPostService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IPostService;
using Post = Crudspa.Education.Publisher.Shared.Contracts.Data.Post;
using PostAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.PostAdded;
using PostRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.PostRemoved;
using PostSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.PostSaved;
using PostSearch = Crudspa.Education.Publisher.Shared.Contracts.Data.PostSearch;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class PostFindForForum : IPaneDisplay, IDisposable
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
    [Inject] public ISessionState SessionState { get; set; } = null!;

    public PostFindForForumModel Model { get; set; } = null!;

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

public class PostFindForForumModel : FindModel<PostSearch, Post>,
    IHandle<PostAdded>, IHandle<PostSaved>, IHandle<PostRemoved>
{
    private readonly IPostService _postService;
    private readonly Guid? _forumId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public PostFindForForumModel(IEventBus eventBus, IScrollService scrollService, IPostService postService, Guid? forumId)
        : base(scrollService)
    {
        _postService = postService;
        _forumId = forumId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Posted",
        ];
    }

    public async Task Handle(PostAdded payload) => await Refresh();
    public async Task Handle(PostSaved payload) => await Refresh();
    public async Task Handle(PostRemoved payload) => await Refresh();

    public List<Orderable> GradeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ClassroomTypeNames
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

        Search.ParentId = _forumId;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = false;
        Search.Grades.Clear();
        Search.Subjects.Clear();

        await WithMany("Initializing...",
            FetchGradeNames(),
            FetchClassroomTypeNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<PostSearch>(Search);
        var response = await WithWaiting("Searching...", () => _postService.SearchForForum(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _postService.Remove(new(new() { Id = id })));
    }

    public async Task FetchGradeNames()
    {
        var response = await WithAlerts(() => _postService.FetchGradeNames(new()), false);
        if (response.Ok) GradeNames = response.Value.ToList();
    }

    public async Task FetchClassroomTypeNames()
    {
        var response = await WithAlerts(() => _postService.FetchClassroomTypeNames(new()), false);
        if (response.Ok) ClassroomTypeNames = response.Value.ToList();
    }
}