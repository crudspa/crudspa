namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class ForumEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IForumService ForumService { get; set; } = null!;

    public ForumEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal");

        Model = new(Path, Id, IsNew, portalId, EventBus, Navigator, ForumService);
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

public class ForumEditModel : EditModel<Forum>, IHandle<ForumSaved>, IHandle<ForumRemoved>, IHandle<ForumsReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly IForumService _forumService;

    public ForumEditModel(String? path, Guid? id, Boolean isNew, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        IForumService forumService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _navigator = navigator;
        _forumService = forumService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ForumSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ForumRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(ForumsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _forumService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title!);
        }
    }

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetForum(new()
            {
                PortalId = _portalId,
                Title = "New Forum",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                Description = String.Empty,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _forumService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetForum(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _forumService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/forum-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _forumService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _forumService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    private void SetForum(Forum forum)
    {
        Entity = forum;
        _navigator.UpdateTitle(_path, Entity.Title!);
    }
}