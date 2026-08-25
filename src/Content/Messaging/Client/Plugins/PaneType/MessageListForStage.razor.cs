namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class MessageListForStage : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IMessageService MessageService { get; set; } = null!;
    [Inject] public IPopulationService PopulationService { get; set; } = null!;

    public MessageListForStageModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, MessageService, PopulationService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class MessageListForStageModel : ListModel<MessageModel>,
    IHandle<MessageAdded>, IHandle<MessageSaved>, IHandle<MessageRemoved>
{
    private readonly IMessageService _messageService;
    private readonly IPopulationService _populationService;
    private readonly Guid? _stageId;

    public MessageListForStageModel(IEventBus eventBus, IScrollService scrollService,
        IMessageService messageService, IPopulationService populationService, Guid? stageId)
        : base(scrollService)
    {
        _messageService = messageService;
        _populationService = populationService;
        _stageId = stageId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(MessageAdded payload) => await Replace(payload.Id, payload.StageId);

    public async Task Handle(MessageSaved payload) => await Replace(payload.Id, payload.StageId);

    public async Task Handle(MessageRemoved payload) => await Rid(payload.Id, payload.StageId);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var response = await WithWaiting("Fetching...",
            () => _messageService.FetchForStage(new(new() { Id = _stageId })), resetAlerts);

        if (!response.Ok)
            return;

        var cards = new List<MessageModel>();

        foreach (var message in response.Value)
            cards.Add(new(message, await FetchPopulationName(message.PopulationId)));

        SetCards(cards);
    }

    public override async Task<Response<MessageModel?>> Fetch(Guid? id)
    {
        var response = await _messageService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new MessageModel(response.Value, await FetchPopulationName(response.Value.PopulationId)))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _messageService.Remove(new(new()
        {
            Id = id,
            StageId = _stageId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_stageId);
    }

    private async Task<String?> FetchPopulationName(Guid? populationId)
    {
        if (populationId is null)
            return null;

        var response = await _populationService.Fetch(new(new() { Id = populationId }));
        return response.Ok ? response.Value.Name : null;
    }
}