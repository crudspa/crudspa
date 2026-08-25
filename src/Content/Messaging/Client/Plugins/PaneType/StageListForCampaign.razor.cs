namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class StageListForCampaign : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IStageService StageService { get; set; } = null!;

    public StageListForCampaignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, StageService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class StageListForCampaignModel : ListOrderablesModel<StageModel>,
    IHandle<StageAdded>, IHandle<StageSaved>, IHandle<StageRemoved>, IHandle<StagesReordered>,
    IHandle<MessageAdded>, IHandle<MessageRemoved>
{
    private readonly IStageService _stageService;
    private readonly Guid? _campaignId;

    public StageListForCampaignModel(IEventBus eventBus, IScrollService scrollService, IStageService stageService, Guid? campaignId)
        : base(scrollService)
    {
        _stageService = stageService;

        _campaignId = campaignId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(StageAdded payload) => await Replace(payload.Id, payload.CampaignId);

    public async Task Handle(StageSaved payload) => await Replace(payload.Id, payload.CampaignId);

    public async Task Handle(StageRemoved payload) => await Rid(payload.Id, payload.CampaignId);

    public async Task Handle(StagesReordered payload) => await Refresh();

    public async Task Handle(MessageAdded payload) => await Replace(payload.StageId, _campaignId);

    public async Task Handle(MessageRemoved payload) => await Replace(payload.StageId, _campaignId);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Campaign>(new() { Id = _campaignId });
        var response = await WithWaiting("Fetching...", () => _stageService.FetchForCampaign(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new StageModel(x)).ToList());
    }

    public override async Task<Response<StageModel?>> Fetch(Guid? id)
    {
        var response = await _stageService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new StageModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _stageService.Remove(new(new()
        {
            Id = id,
            CampaignId = _campaignId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_campaignId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Stage).ToList();
        return await WithWaiting("Saving...", () => _stageService.SaveOrder(new(orderables)));
    }
}

public class StageModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleStageChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Stage));

    private Stage _stage;

    public String? Name => Stage.Name;

    public StageModel(Stage stage)
    {
        _stage = stage;
        _stage.PropertyChanged += HandleStageChanged;
    }

    public void Dispose()
    {
        _stage.PropertyChanged -= HandleStageChanged;
    }

    public Guid? Id
    {
        get => _stage.Id;
        set => _stage.Id = value;
    }

    public Int32? Ordinal
    {
        get => _stage.Ordinal;
        set => _stage.Ordinal = value;
    }

    public Stage Stage
    {
        get => _stage;
        set => SetProperty(ref _stage, value);
    }
}