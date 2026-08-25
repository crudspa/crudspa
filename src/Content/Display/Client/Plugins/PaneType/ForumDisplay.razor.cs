using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class ForumDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IForumRunService ForumRunService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;

    public ForumDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, ForumRunService);
        Model.PropertyChanged += HandleModelChanged;
        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ForumDisplayModel : ScreenModel, IHandle<ForumRunChanged>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly IEventBus _eventBus;
    private readonly INavigator _navigator;
    private readonly IForumRunService _forumRunService;

    public ForumDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IForumRunService forumRunService)
    {
        _path = path;
        _id = id;
        _eventBus = eventBus;
        _navigator = navigator;
        _forumRunService = forumRunService;

        Search.ParentId = id;
        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 25;
        Search.Sort.Field = "Activity";
        Search.Sort.Ascending = false;
        Search.PostedRange.Type = DateRange.Types.Any;

        _eventBus.Subscribe(this);
    }

    public Forum? Forum
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Unavailable
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Thread> Threads
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ThreadSearch Search { get; } = new();

    public Boolean CreatingThread
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Thread? DraftThread
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override void Dispose()
    {
        _eventBus.Unsubscribe(this);
        Search.Dispose();
        base.Dispose();
    }

    public async Task Initialize()
    {
        var response = await WithWaiting("Loading forum...", () => _forumRunService.FetchForum(new(new() { Id = _id })));
        if (!response.Ok)
        {
            if (response.Errors.IsEmpty())
            {
                Alerts.Clear();
                Unavailable = true;
                _navigator.UpdateTitle(_path, "Forum unavailable");
            }

            return;
        }

        Forum = response.Value;
        if (Forum is null)
        {
            Unavailable = true;
            _navigator.UpdateTitle(_path, "Forum unavailable");
            return;
        }

        _navigator.UpdateTitle(_path, Forum.Title!);
        await RefreshThreads();
    }

    public async Task RefreshThreads()
    {
        var response = await WithWaiting("Loading threads...", () => _forumRunService.SearchThreads(new(Search)));
        if (!response.Ok) return;

        Threads = response.Value.ToObservable();
        Search.Paged.TotalCount = response.Value.FirstOrDefault()?.TotalCount ?? 0;
    }

    public void BeginThread()
    {
        DraftThread = new()
        {
            ForumId = _id,
            Title = String.Empty,
            Pinned = false,
            Comment = new() { Body = String.Empty },
            ForumBundles = Forum?.ForumBundles.DeepClone() ?? [],
        };
        CreatingThread = true;
    }

    public void CancelThread()
    {
        DraftThread = null;
        CreatingThread = false;
    }

    public async Task AddThread()
    {
        if (DraftThread is null) return;
        var response = await WithWaiting("Posting thread...", () => _forumRunService.AddThread(new(DraftThread)));
        if (!response.Ok || response.Value?.Id is null) return;

        CancelThread();
        _navigator.GoTo($"{_path}/thread-{response.Value.Id:D}");
    }

    public String ThreadPath(Guid? id) => $"{_path}/thread-{id:D}";

    public void GoToThread(Guid? id) => _navigator.GoTo(ThreadPath(id));

    public async Task HandlePageNumberChanged(Int32 pageNumber)
    {
        Search.Paged.PageNumber = pageNumber;
        await RefreshThreads();
    }

    public async Task Handle(ForumRunChanged payload)
    {
        if (payload.ForumId == _id && !payload.ThreadId.HasValue && !payload.CommentId.HasValue)
            await Initialize();
        else if (payload.ForumId == _id || Threads.Any(x => x.Id == payload.ThreadId))
            await RefreshThreads();
    }
}