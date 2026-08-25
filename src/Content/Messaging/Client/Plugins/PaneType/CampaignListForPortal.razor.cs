namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class CampaignListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ICampaignService CampaignService { get; set; } = null!;

    public CampaignListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, CampaignService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CampaignListForPortalModel : ListModel<CampaignModel>,
    IHandle<CampaignAdded>, IHandle<CampaignSaved>, IHandle<CampaignRemoved>
{
    private readonly ICampaignService _campaignService;
    private readonly Guid? _portalId;

    public CampaignListForPortalModel(IEventBus eventBus, IScrollService scrollService, ICampaignService campaignService, Guid? portalId)
        : base(scrollService)
    {
        _campaignService = campaignService;

        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CampaignAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(CampaignSaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(CampaignRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _campaignService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new CampaignModel(x)).ToList());
    }

    public override async Task<Response<CampaignModel?>> Fetch(Guid? id)
    {
        var response = await _campaignService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new CampaignModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _campaignService.Remove(new(new()
        {
            Id = id,
            PortalId = _portalId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_portalId);
    }
}

public class CampaignModel : Observable, IDisposable, INamed
{
    private void HandleCampaignChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Campaign));

    private Campaign _campaign;

    public String? Name => Campaign.Name;

    public CampaignModel(Campaign campaign)
    {
        _campaign = campaign;
        _campaign.PropertyChanged += HandleCampaignChanged;
    }

    public void Dispose()
    {
        _campaign.PropertyChanged -= HandleCampaignChanged;
    }

    public Guid? Id
    {
        get => _campaign.Id;
        set => _campaign.Id = value;
    }

    public Campaign Campaign
    {
        get => _campaign;
        set => SetProperty(ref _campaign, value);
    }
}