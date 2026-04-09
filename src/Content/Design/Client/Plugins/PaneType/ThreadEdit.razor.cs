using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class ThreadEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IThreadService ThreadService { get; set; } = null!;

    public ThreadEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var forumId = Path!.Id("forum");

        Model = new(Path, Id, IsNew, forumId, EventBus, Navigator, ThreadService);
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

public class ThreadEditModel : EditModel<Thread>, IHandle<ThreadSaved>, IHandle<ThreadRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _forumId;
    private readonly INavigator _navigator;
    private readonly IThreadService _threadService;

    public ThreadEditModel(String? path, Guid? id, Boolean isNew, Guid? forumId,
        IEventBus eventBus,
        INavigator navigator,
        IThreadService threadService) : base(isNew)
    {
        _path = path;
        _id = id;
        _forumId = forumId;
        _navigator = navigator;
        _threadService = threadService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ThreadSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ThreadRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetThread(new()
            {
                ForumId = _forumId,
                Title = "New Thread",
                Pinned = false,
                Comment = new()
                {
                    Body = String.Empty,
                },
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _threadService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetThread(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _threadService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/thread-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _threadService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void SetThread(Thread thread)
    {
        Entity = thread;
        _navigator.UpdateTitle(_path, Entity.Title!);
    }
}