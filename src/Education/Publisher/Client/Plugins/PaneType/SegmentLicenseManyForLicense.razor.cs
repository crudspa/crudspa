namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using License = Shared.Contracts.Data.License;
using SegmentAdded = Crudspa.Framework.Core.Shared.Contracts.Events.SegmentAdded;
using SegmentSaved = Crudspa.Framework.Core.Shared.Contracts.Events.SegmentSaved;
using SegmentRemoved = Crudspa.Framework.Core.Shared.Contracts.Events.SegmentRemoved;

public partial class SegmentLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISegmentLicenseService SegmentLicenseService { get; set; } = null!;

    public SegmentLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SegmentLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SegmentLicenseManyForLicenseModel : ManyModel<SegmentLicenseModel>,
    IHandle<SegmentLicenseAdded>, IHandle<SegmentLicenseSaved>, IHandle<SegmentLicenseRemoved>,
    IHandle<SegmentAdded>, IHandle<SegmentSaved>, IHandle<SegmentRemoved>, IHandle<SegmentsReordered>
{
    private readonly ISegmentLicenseService _segmentLicenseService;
    private readonly Guid? _licenseId;

    public SegmentLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, ISegmentLicenseService segmentLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _segmentLicenseService = segmentLicenseService;

        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SegmentLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(SegmentLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(SegmentLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(SegmentAdded payload) => await FetchSegmentNames();

    public async Task Handle(SegmentSaved payload) => await FetchSegmentNames();

    public async Task Handle(SegmentRemoved payload) => await FetchSegmentNames();

    public async Task Handle(SegmentsReordered payload) => await FetchSegmentNames();

    public ObservableCollection<Orderable> SegmentNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchSegmentNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _segmentLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new SegmentLicenseModel(x)));
    }

    public override async Task Create()
    {
        var segmentLicense = new SegmentLicense
        {
            Id = Guid.NewGuid(),
            SegmentId = SegmentNames.MinBy(x => x.Ordinal)?.Id,
            LicenseId = _licenseId,
        };

        var form = await CreateForm(new(segmentLicense));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<SegmentLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _segmentLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new SegmentLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<SegmentLicenseModel?>> Add(FormModel<SegmentLicenseModel> form)
    {
        var response = await _segmentLicenseService.Add(new(form.Entity.SegmentLicense));

        return response.Ok
            ? new(new SegmentLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<SegmentLicenseModel> form)
    {
        var segmentLicense = form.Entity.SegmentLicense;

        return await _segmentLicenseService.Save(new(segmentLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _segmentLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchSegmentNames()
    {
        var response = await WithAlerts(() => _segmentLicenseService.FetchSegmentNames(new()), false);
        if (response.Ok) SegmentNames = response.Value.ToObservable();
    }
}

public class SegmentLicenseModel : Observable, IDisposable, INamed
{
    private void HandleSegmentLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(SegmentLicense));

    private SegmentLicense _segmentLicense;

    public String? Name => SegmentLicense.SegmentTitle;

    public SegmentLicenseModel(SegmentLicense segmentLicense)
    {
        _segmentLicense = segmentLicense;
        _segmentLicense.PropertyChanged += HandleSegmentLicenseChanged;
    }

    public void Dispose()
    {
        _segmentLicense.PropertyChanged -= HandleSegmentLicenseChanged;
    }

    public Guid? Id
    {
        get => _segmentLicense.Id;
        set => _segmentLicense.Id = value;
    }

    public SegmentLicense SegmentLicense
    {
        get => _segmentLicense;
        set => SetProperty(ref _segmentLicense, value);
    }
}