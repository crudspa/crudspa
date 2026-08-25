namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class ForumList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IForumRunService ForumRunService { get; set; } = null!;

    public ForumListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, EventBus, Navigator, ForumRunService);
        Model.PropertyChanged += HandleModelChanged;
        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ForumListModel : ScreenModel, IHandle<ForumRunChanged>
{
    private readonly String? _path;
    private readonly IEventBus _eventBus;
    private readonly INavigator _navigator;
    private readonly IForumRunService _forumRunService;

    public ForumListModel(String? path,
        IEventBus eventBus,
        INavigator navigator,
        IForumRunService forumRunService)
    {
        _path = path;
        _eventBus = eventBus;
        _navigator = navigator;
        _forumRunService = forumRunService;

        _eventBus.Subscribe(this);
    }

    public ObservableCollection<Forum>? Forums
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading forums...", () => _forumRunService.FetchForums(new()));
        if (response.Ok) Forums = response.Value.ToObservable();
    }

    public String ForumPath(Guid? id) => $"{_path}/forum-{id:D}";

    public void GoToForum(Guid? id) => _navigator.GoTo(ForumPath(id));

    public async Task Handle(ForumRunChanged payload)
    {
        if (!payload.ThreadId.HasValue && !payload.CommentId.HasValue)
            await Refresh();
    }

    public override void Dispose()
    {
        _eventBus.Unsubscribe(this);
        base.Dispose();
    }
}