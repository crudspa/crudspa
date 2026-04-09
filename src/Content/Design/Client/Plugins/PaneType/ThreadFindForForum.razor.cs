using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class ThreadFindForForum : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IThreadService ThreadService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public ThreadFindForForumModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ThreadService, Id);
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
        Navigator.GoTo($"{Path}/thread-{Guid.NewGuid():D}?state=new");
    }
}

public class ThreadFindForForumModel : FindModel<ThreadSearch, Thread>,
    IHandle<ThreadAdded>, IHandle<ThreadSaved>, IHandle<ThreadRemoved>,
    IHandle<CommentAdded>, IHandle<CommentRemoved>
{
    private readonly IThreadService _threadService;
    private readonly Guid? _forumId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public ThreadFindForForumModel(IEventBus eventBus, IScrollService scrollService, IThreadService threadService, Guid? forumId)
        : base(scrollService)
    {
        _threadService = threadService;
        _forumId = forumId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Posted",
            "Title",
        ];
    }

    public async Task Handle(ThreadAdded payload) => await Refresh();
    public async Task Handle(ThreadSaved payload) => await Refresh();
    public async Task Handle(ThreadRemoved payload) => await Refresh();
    public async Task Handle(CommentAdded payload) => await Refresh();
    public async Task Handle(CommentRemoved payload) => await Refresh();

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
        Search.PostedRange.Type = DateRange.Types.Any;
        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<ThreadSearch>(Search);
        var response = await WithWaiting("Searching...", () => _threadService.SearchForForum(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _threadService.Remove(new(new() { Id = id })));
    }
}