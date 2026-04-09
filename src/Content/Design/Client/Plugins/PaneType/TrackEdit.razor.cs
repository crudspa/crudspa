namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class TrackEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ITrackService TrackService { get; set; } = null!;

    public TrackEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal");

        Model = new(Path, Id, IsNew, portalId, EventBus, Navigator, TrackService);
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

public class TrackEditModel : EditModel<Track>,
    IHandle<TrackSaved>, IHandle<TrackRemoved>, IHandle<TracksReordered>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly ITrackService _trackService;

    public TrackEditModel(String? path, Guid? id, Boolean isNew, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        ITrackService trackService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _navigator = navigator;
        _trackService = trackService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(TrackSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(TrackRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(TracksReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _trackService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title);
        }
    }

    public async Task Handle(AchievementAdded payload) => await FetchAchievementNames();

    public async Task Handle(AchievementSaved payload) => await FetchAchievementNames();

    public async Task Handle(AchievementRemoved payload) => await FetchAchievementNames();

    public ObservableCollection<Named> AchievementNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchAchievementNames(),
            FetchContentStatusNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var track = new Track
            {
                PortalId = _portalId,
                Title = "New Track",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                Description = String.Empty,
                RequireSequentialCompletion = true,
            };

            SetTrack(track);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _trackService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetTrack(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _trackService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/track-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _trackService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchAchievementNames()
    {
        var response = await WithAlerts(() => _trackService.FetchAchievementNames(new(new() { Id = _portalId })), false);
        if (response.Ok) AchievementNames = response.Value.ToObservable();
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _trackService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    private void SetTrack(Track track)
    {
        Entity = track;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}