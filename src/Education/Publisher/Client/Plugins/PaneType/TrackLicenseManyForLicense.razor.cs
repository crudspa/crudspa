namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using License = Shared.Contracts.Data.License;
using TrackAdded = Crudspa.Content.Design.Shared.Contracts.Events.TrackAdded;
using TrackSaved = Crudspa.Content.Design.Shared.Contracts.Events.TrackSaved;
using TrackRemoved = Crudspa.Content.Design.Shared.Contracts.Events.TrackRemoved;

public partial class TrackLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ITrackLicenseService TrackLicenseService { get; set; } = null!;

    public TrackLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, TrackLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class TrackLicenseManyForLicenseModel : ManyModel<TrackLicenseModel>,
    IHandle<TrackLicenseAdded>, IHandle<TrackLicenseSaved>, IHandle<TrackLicenseRemoved>,
    IHandle<TrackAdded>, IHandle<TrackSaved>, IHandle<TrackRemoved>, IHandle<TracksReordered>
{
    private readonly ITrackLicenseService _trackLicenseService;
    private readonly Guid? _licenseId;

    public TrackLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, ITrackLicenseService trackLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _trackLicenseService = trackLicenseService;

        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(TrackLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(TrackLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(TrackLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(TrackAdded payload) => await FetchTrackNames();

    public async Task Handle(TrackSaved payload) => await FetchTrackNames();

    public async Task Handle(TrackRemoved payload) => await FetchTrackNames();

    public async Task Handle(TracksReordered payload) => await FetchTrackNames();

    public ObservableCollection<Orderable> TrackNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchTrackNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _trackLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new TrackLicenseModel(x)));
    }

    public override async Task Create()
    {
        var trackLicense = new TrackLicense
        {
            Id = Guid.NewGuid(),
            TrackId = TrackNames.MinBy(x => x.Ordinal)?.Id,
            LicenseId = _licenseId,
        };

        var form = await CreateForm(new(trackLicense));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<TrackLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _trackLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new TrackLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<TrackLicenseModel?>> Add(FormModel<TrackLicenseModel> form)
    {
        var response = await _trackLicenseService.Add(new(form.Entity.TrackLicense));

        return response.Ok
            ? new(new TrackLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<TrackLicenseModel> form)
    {
        var trackLicense = form.Entity.TrackLicense;

        return await _trackLicenseService.Save(new(trackLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _trackLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchTrackNames()
    {
        var response = await WithAlerts(() => _trackLicenseService.FetchTrackNames(new()), false);
        if (response.Ok) TrackNames = response.Value.ToObservable();
    }
}

public class TrackLicenseModel : Observable, IDisposable, INamed
{
    private void HandleTrackLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(TrackLicense));

    private TrackLicense _trackLicense;

    public String? Name => TrackLicense.TrackTitle;

    public TrackLicenseModel(TrackLicense trackLicense)
    {
        _trackLicense = trackLicense;
        _trackLicense.PropertyChanged += HandleTrackLicenseChanged;
    }

    public void Dispose()
    {
        _trackLicense.PropertyChanged -= HandleTrackLicenseChanged;
    }

    public Guid? Id
    {
        get => _trackLicense.Id;
        set => _trackLicense.Id = value;
    }

    public TrackLicense TrackLicense
    {
        get => _trackLicense;
        set => SetProperty(ref _trackLicense, value);
    }
}