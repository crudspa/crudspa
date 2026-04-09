using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class DistrictLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IDistrictLicenseService DistrictLicenseService { get; set; } = null!;

    public DistrictLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, DistrictLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class DistrictLicenseManyForLicenseModel : ManyModel<DistrictLicenseModel>,
    IHandle<DistrictLicenseAdded>, IHandle<DistrictLicenseSaved>, IHandle<DistrictLicenseRemoved>, IHandle<DistrictLicenseRelationsSaved>
{
    private readonly IDistrictLicenseService _districtLicenseService;
    private readonly Guid? _licenseId;

    public DistrictLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, IDistrictLicenseService districtLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _districtLicenseService = districtLicenseService;
        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(DistrictLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(DistrictLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(DistrictLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(DistrictLicenseRelationsSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public ObservableCollection<Named> DistrictNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchDistrictNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _districtLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new DistrictLicenseModel(x)));
    }

    public override async Task Create()
    {
        var districtLicense = new DistrictLicense
        {
            Id = Guid.NewGuid(),
            LicenseId = _licenseId,
            DistrictId = DistrictNames.FirstOrDefault()?.Id,
            AllSchools = true,
        };

        var form = await CreateForm(new(districtLicense));
        form.Entity.SelectingDistrict = true;
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<DistrictLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _districtLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new DistrictLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<DistrictLicenseModel?>> Add(FormModel<DistrictLicenseModel> form)
    {
        var response = await _districtLicenseService.Add(new(form.Entity.DistrictLicense));

        return response.Ok
            ? new(new DistrictLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<DistrictLicenseModel> form)
    {
        var districtLicense = form.Entity.DistrictLicense;

        if (!form.Entity.SelectingDistrict)
            return await _districtLicenseService.SaveRelations(new(districtLicense));

        return await _districtLicenseService.Save(new(districtLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _districtLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchDistrictNames()
    {
        var response = await WithAlerts(() => _districtLicenseService.FetchDistrictNames(new()), false);
        if (response.Ok) DistrictNames = response.Value.ToObservable();
    }
}

public class DistrictLicenseModel : Observable, IDisposable, INamed
{
    private void HandleDistrictLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(DistrictLicense));

    private DistrictLicense _districtLicense;
    private Guid? _previousDistrictId;

    public String? Name => DistrictLicense.DistrictName;

    public DistrictLicenseModel(DistrictLicense districtLicense)
    {
        _districtLicense = districtLicense;
        _districtLicense.PropertyChanged += HandleDistrictLicenseChanged;
    }

    public void Dispose()
    {
        _districtLicense.PropertyChanged -= HandleDistrictLicenseChanged;
    }

    public Guid? Id
    {
        get => _districtLicense.Id;
        set => _districtLicense.Id = value;
    }

    public DistrictLicense DistrictLicense
    {
        get => _districtLicense;
        set => SetProperty(ref _districtLicense, value);
    }

    public Boolean SelectingDistrict
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void StartDistrictSelection()
    {
        _previousDistrictId = DistrictLicense.DistrictId;
        SelectingDistrict = true;
    }

    public void CancelDistrictSelection()
    {
        DistrictLicense.DistrictId = _previousDistrictId;
        SelectingDistrict = false;
    }
}