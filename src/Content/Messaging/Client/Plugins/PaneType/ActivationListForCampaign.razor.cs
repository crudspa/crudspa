namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class ActivationListForCampaign : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IActivationService ActivationService { get; set; } = null!;

    public ActivationListForCampaignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ActivationService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void StartActivation() => Navigator.GoTo($"{Path}/activate");
}

public class ActivationListForCampaignModel : ListModel<ActivationModel>,
    IHandle<ActivationAdded>, IHandle<ActivationSaved>, IHandle<ActivationRemoved>
{
    private readonly IActivationService _activationService;
    private readonly Guid? _campaignId;

    public ActivationListForCampaignModel(IEventBus eventBus, IScrollService scrollService, IActivationService activationService, Guid? campaignId)
        : base(scrollService)
    {
        _activationService = activationService;

        _campaignId = campaignId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ActivationAdded payload) => await Replace(payload.Id, payload.CampaignId);

    public async Task Handle(ActivationSaved payload) => await Replace(payload.Id, payload.CampaignId);

    public async Task Handle(ActivationRemoved payload) => await Rid(payload.Id, payload.CampaignId);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Campaign>(new() { Id = _campaignId });
        var response = await WithWaiting("Fetching...", () => _activationService.FetchForCampaign(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ActivationModel(x)).ToList());
    }

    public override async Task<Response<ActivationModel?>> Fetch(Guid? id)
    {
        var response = await _activationService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ActivationModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _activationService.Remove(new(new()
        {
            Id = id,
            CampaignId = _campaignId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_campaignId);
    }
}

public class ActivationModel : Observable, IDisposable, INamed
{
    private void HandleActivationChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Activation));

    private Activation _activation;

    public String? Name => Activation.CampaignName;

    public ActivationModel(Activation activation)
    {
        _activation = activation;
        _activation.PropertyChanged += HandleActivationChanged;
    }

    public void Dispose()
    {
        _activation.PropertyChanged -= HandleActivationChanged;
    }

    public Guid? Id
    {
        get => _activation.Id;
        set => _activation.Id = value;
    }

    public Activation Activation
    {
        get => _activation;
        set => SetProperty(ref _activation, value);
    }
}