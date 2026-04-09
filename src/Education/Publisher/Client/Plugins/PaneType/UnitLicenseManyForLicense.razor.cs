using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class UnitLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IUnitLicenseService UnitLicenseService { get; set; } = null!;

    public UnitLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, UnitLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class UnitLicenseManyForLicenseModel : ManyModel<UnitLicenseModel>,
    IHandle<UnitLicenseAdded>, IHandle<UnitLicenseSaved>, IHandle<UnitLicenseRemoved>, IHandle<UnitLicenseRelationsSaved>
{
    private readonly IUnitLicenseService _unitLicenseService;
    private readonly Guid? _licenseId;

    public UnitLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, IUnitLicenseService unitLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _unitLicenseService = unitLicenseService;
        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(UnitLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(UnitLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(UnitLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(UnitLicenseRelationsSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public ObservableCollection<Orderable> UnitNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchUnitNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _unitLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new UnitLicenseModel(x)));
    }

    public override async Task Create()
    {
        var unitLicense = new UnitLicense
        {
            Id = Guid.NewGuid(),
            LicenseId = _licenseId,
            UnitId = UnitNames.MinBy(x => x.Ordinal)?.Id,
            AllBooks = true,
            AllLessons = true,
        };

        var form = await CreateForm(new(unitLicense));
        form.Entity.SelectingUnit = true;
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<UnitLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _unitLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new UnitLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<UnitLicenseModel?>> Add(FormModel<UnitLicenseModel> form)
    {
        var response = await _unitLicenseService.Add(new(form.Entity.UnitLicense));

        return response.Ok
            ? new(new UnitLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<UnitLicenseModel> form)
    {
        var unitLicense = form.Entity.UnitLicense;

        if (!form.Entity.SelectingUnit)
            return await _unitLicenseService.SaveRelations(new(unitLicense));

        return await _unitLicenseService.Save(new(unitLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _unitLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchUnitNames()
    {
        var response = await WithAlerts(() => _unitLicenseService.FetchUnitNames(new()), false);
        if (response.Ok) UnitNames = response.Value.ToObservable();
    }
}

public class UnitLicenseModel : Observable, IDisposable, INamed
{
    private void HandleUnitLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(UnitLicense));

    private UnitLicense _unitLicense;
    private Guid? _previousUnitId;

    public String? Name => UnitLicense.UnitTitle;

    public UnitLicenseModel(UnitLicense unitLicense)
    {
        _unitLicense = unitLicense;
        _unitLicense.PropertyChanged += HandleUnitLicenseChanged;
    }

    public void Dispose()
    {
        _unitLicense.PropertyChanged -= HandleUnitLicenseChanged;
    }

    public Guid? Id
    {
        get => _unitLicense.Id;
        set => _unitLicense.Id = value;
    }

    public UnitLicense UnitLicense
    {
        get => _unitLicense;
        set => SetProperty(ref _unitLicense, value);
    }

    public Boolean SelectingUnit
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void StartUnitSelection()
    {
        _previousUnitId = UnitLicense.UnitId;
        SelectingUnit = true;
    }

    public void CancelUnitSelection()
    {
        UnitLicense.UnitId = _previousUnitId;
        SelectingUnit = false;
    }
}