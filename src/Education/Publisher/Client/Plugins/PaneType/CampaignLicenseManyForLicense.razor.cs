namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using License = Shared.Contracts.Data.License;
using CampaignAdded = Crudspa.Content.Messaging.Shared.Contracts.Events.CampaignAdded;
using CampaignSaved = Crudspa.Content.Messaging.Shared.Contracts.Events.CampaignSaved;
using CampaignRemoved = Crudspa.Content.Messaging.Shared.Contracts.Events.CampaignRemoved;

public partial class CampaignLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ICampaignLicenseService CampaignLicenseService { get; set; } = null!;

    public CampaignLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, CampaignLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CampaignLicenseManyForLicenseModel : ManyModel<CampaignLicenseModel>,
    IHandle<CampaignLicenseAdded>, IHandle<CampaignLicenseSaved>, IHandle<CampaignLicenseRemoved>,
    IHandle<CampaignAdded>, IHandle<CampaignSaved>, IHandle<CampaignRemoved>
{
    private readonly ICampaignLicenseService _campaignLicenseService;
    private readonly Guid? _licenseId;

    public CampaignLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, ICampaignLicenseService campaignLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _campaignLicenseService = campaignLicenseService;

        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CampaignLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(CampaignLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(CampaignLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(CampaignAdded payload) => await FetchCampaignNames();

    public async Task Handle(CampaignSaved payload) => await FetchCampaignNames();

    public async Task Handle(CampaignRemoved payload) => await FetchCampaignNames();

    public ObservableCollection<Named> CampaignNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchCampaignNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _campaignLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new CampaignLicenseModel(x)));
    }

    public override async Task Create()
    {
        var campaignLicense = new CampaignLicense
        {
            Id = Guid.NewGuid(),
            CampaignId = CampaignNames.MinBy(x => x.Name)?.Id,
            LicenseId = _licenseId,
        };

        var form = await CreateForm(new(campaignLicense));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<CampaignLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _campaignLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new CampaignLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<CampaignLicenseModel?>> Add(FormModel<CampaignLicenseModel> form)
    {
        var response = await _campaignLicenseService.Add(new(form.Entity.CampaignLicense));

        return response.Ok
            ? new(new CampaignLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<CampaignLicenseModel> form)
    {
        var campaignLicense = form.Entity.CampaignLicense;

        return await _campaignLicenseService.Save(new(campaignLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _campaignLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchCampaignNames()
    {
        var response = await WithAlerts(() => _campaignLicenseService.FetchCampaignNames(new()), false);
        if (response.Ok) CampaignNames = response.Value.ToObservable();
    }
}

public class CampaignLicenseModel : Observable, IDisposable, INamed
{
    private void HandleCampaignLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(CampaignLicense));

    private CampaignLicense _campaignLicense;

    public String? Name => CampaignLicense.CampaignName;

    public CampaignLicenseModel(CampaignLicense campaignLicense)
    {
        _campaignLicense = campaignLicense;
        _campaignLicense.PropertyChanged += HandleCampaignLicenseChanged;
    }

    public void Dispose()
    {
        _campaignLicense.PropertyChanged -= HandleCampaignLicenseChanged;
    }

    public Guid? Id
    {
        get => _campaignLicense.Id;
        set => _campaignLicense.Id = value;
    }

    public CampaignLicense CampaignLicense
    {
        get => _campaignLicense;
        set => SetProperty(ref _campaignLicense, value);
    }
}