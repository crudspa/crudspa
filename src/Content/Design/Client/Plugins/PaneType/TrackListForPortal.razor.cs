namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class TrackListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ITrackService TrackService { get; set; } = null!;

    public TrackListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, TrackService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class TrackListForPortalModel : ListOrderablesModel<TrackModel>,
    IHandle<TrackAdded>, IHandle<TrackSaved>, IHandle<TrackRemoved>, IHandle<TracksReordered>,
    IHandle<CourseAdded>, IHandle<CourseRemoved>
{
    private readonly ITrackService _trackService;
    private readonly Guid? _portalId;

    public TrackListForPortalModel(IEventBus eventBus, IScrollService scrollService, ITrackService trackService, Guid? portalId)
        : base(scrollService)
    {
        _trackService = trackService;

        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(TrackAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(TrackSaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(TrackRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public async Task Handle(TracksReordered payload) => await Refresh();

    public async Task Handle(CourseAdded payload)
    {
        if (payload.TrackId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.TrackId)))
            await Replace(payload.TrackId);
    }

    public async Task Handle(CourseRemoved payload)
    {
        if (payload.TrackId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.TrackId)))
            await Replace(payload.TrackId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _trackService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new TrackModel(x)).ToList());
    }

    public override async Task<Response<TrackModel?>> Fetch(Guid? id)
    {
        var response = await _trackService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new TrackModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _trackService.Remove(new(new()
        {
            Id = id,
            PortalId = _portalId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_portalId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Track).ToList();
        return await WithWaiting("Saving...", () => _trackService.SaveOrder(new(orderables)));
    }
}

public class TrackModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleTrackChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Track));

    private Track _track;

    public String? Name => Track.Title;

    public TrackModel(Track track)
    {
        _track = track;
        _track.PropertyChanged += HandleTrackChanged;
    }

    public void Dispose()
    {
        _track.PropertyChanged -= HandleTrackChanged;
    }

    public Guid? Id
    {
        get => _track.Id;
        set => _track.Id = value;
    }

    public Int32? Ordinal
    {
        get => _track.Ordinal;
        set => _track.Ordinal = value;
    }

    public Track Track
    {
        get => _track;
        set => SetProperty(ref _track, value);
    }
}