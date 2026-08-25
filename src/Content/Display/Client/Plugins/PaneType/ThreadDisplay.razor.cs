using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class ThreadDisplay : IPaneDisplay, IDisposable
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

    public ThreadDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, ForumRunService);
        Model.PropertyChanged += HandleModelChanged;
        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ThreadDisplayModel : ScreenModel, IHandle<ForumRunChanged>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly IEventBus _eventBus;
    private readonly INavigator _navigator;
    private readonly IForumRunService _forumRunService;

    public ThreadDisplayModel(String? path,
        Guid? id,
        IEventBus eventBus,
        INavigator navigator,
        IForumRunService forumRunService)
    {
        _path = path;
        _id = id;
        _eventBus = eventBus;
        _navigator = navigator;
        _forumRunService = forumRunService;

        _eventBus.Subscribe(this);
    }

    public IReadOnlyList<Emoji> ReactionOptions { get; } = Emoji.Reactions();

    public Thread? Thread
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Unavailable
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Comment> Comments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Thread? DraftThread
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Comment? Reply
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean EditingThread
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Replying
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean ConfirmingThreadDelete
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var threadResponse = await WithWaiting("Loading thread...", () => _forumRunService.FetchThread(new(new() { Id = _id })));
        if (!threadResponse.Ok)
        {
            if (threadResponse.Errors.IsEmpty())
            {
                Alerts.Clear();
                Unavailable = true;
                _navigator.UpdateTitle(_path, "Thread unavailable");
            }

            return;
        }

        Thread = threadResponse.Value;
        if (Thread is null)
        {
            Unavailable = true;
            _navigator.UpdateTitle(_path, "Thread unavailable");
            return;
        }

        _navigator.UpdateTitle(_path, Thread.Title!);

        var commentsResponse = await WithWaiting("Loading comments...", () => _forumRunService.FetchComments(new(new() { Id = _id })));
        if (commentsResponse.Ok) Comments = commentsResponse.Value.ToObservable();
    }

    public void BeginThreadEdit()
    {
        DraftThread = Thread?.DeepClone();
        EditingThread = DraftThread is not null;
    }

    public void CancelThread()
    {
        DraftThread = null;
        EditingThread = false;
    }

    public async Task SaveThread()
    {
        if (DraftThread is null) return;
        var response = await WithWaiting("Saving thread...", () => _forumRunService.SaveThread(new(DraftThread)));
        if (!response.Ok) return;

        CancelThread();
        await Refresh();
    }

    public async Task DeleteThread()
    {
        if (Thread is null) return;
        var response = await WithWaiting("Deleting thread...", () => _forumRunService.RemoveThread(new(new()
        {
            Id = Thread.Id,
            ForumId = Thread.ForumId,
        })));

        if (response.Ok) _navigator.GoTo(_path.Parent());
    }

    public void RequestThreadDelete() => ConfirmingThreadDelete = true;

    public void CancelThreadDelete() => ConfirmingThreadDelete = false;

    public void BeginReply()
    {
        Reply = new()
        {
            ThreadId = Thread?.Id,
            Body = String.Empty,
            ForumBundles = Thread?.ForumBundles.DeepClone() ?? [],
        };
        Replying = true;
    }

    public void CancelReply()
    {
        Reply = null;
        Replying = false;
    }

    public async Task AddReply()
    {
        if (Reply is null) return;
        var response = await WithWaiting("Posting reply...", () => _forumRunService.AddComment(new(Reply)));
        if (!response.Ok) return;

        CancelReply();
        await Refresh();
    }

    public async Task React(String? emoji)
    {
        if (Thread?.Comment.Id is null) return;
        var selected = Thread.Comment.Reactions.FirstOrDefault(x => x.Selected)?.Emoji;
        var response = await WithWaiting("Saving reaction...", () => _forumRunService.SetReaction(new(new()
        {
            CommentId = Thread.Comment.Id,
            Emoji = emoji.IsBasically(selected) ? null : emoji,
        })));

        if (response.Ok) await Refresh();
    }

    public async Task Handle(ForumRunChanged payload)
    {
        if (payload.ThreadId == _id || ContainsComment(payload.CommentId))
            await Refresh();
    }

    public override void Dispose()
    {
        _eventBus.Unsubscribe(this);
        base.Dispose();
    }

    private Boolean ContainsComment(Guid? commentId)
    {
        if (!commentId.HasValue)
            return false;

        return Thread?.Comment.Id == commentId || ContainsComment(Comments, commentId);
    }

    private static Boolean ContainsComment(IEnumerable<Comment> comments, Guid? commentId) =>
        comments.Any(x => x.Id == commentId || ContainsComment(x.Children, commentId));
}