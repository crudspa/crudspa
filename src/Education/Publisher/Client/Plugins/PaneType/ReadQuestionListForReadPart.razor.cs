namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ReadQuestionListForReadPart : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IReadQuestionService ReadQuestionService { get; set; } = null!;

    public ReadQuestionListForReadPartModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ReadQuestionService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ReadQuestionListForReadPartModel : ListOrderablesModel<ReadQuestionModel>,
    IHandle<ReadQuestionAdded>, IHandle<ReadQuestionSaved>, IHandle<ReadQuestionRemoved>, IHandle<ReadQuestionsReordered>
{
    private readonly IReadQuestionService _readQuestionService;
    private readonly Guid? _readPartId;

    public ReadQuestionListForReadPartModel(IEventBus eventBus, IScrollService scrollService, IReadQuestionService readQuestionService, Guid? readPartId)
        : base(scrollService)
    {
        _readQuestionService = readQuestionService;
        _readPartId = readPartId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ReadQuestionAdded payload) => await Replace(payload.Id, payload.ReadPartId);

    public async Task Handle(ReadQuestionSaved payload) => await Replace(payload.Id, payload.ReadPartId);

    public async Task Handle(ReadQuestionRemoved payload) => await Rid(payload.Id, payload.ReadPartId);

    public async Task Handle(ReadQuestionsReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<ReadPart>(new() { Id = _readPartId });
        var response = await WithWaiting("Fetching...", () => _readQuestionService.FetchForReadPart(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ReadQuestionModel(x)).ToList());
    }

    public override async Task<Response<ReadQuestionModel?>> Fetch(Guid? id)
    {
        var response = await _readQuestionService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ReadQuestionModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _readQuestionService.Remove(new(new()
        {
            Id = id,
            ReadPartId = _readPartId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_readPartId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.ReadQuestion).ToList();
        return await WithWaiting("Saving...", () => _readQuestionService.SaveOrder(new(orderables)));
    }
}

public class ReadQuestionModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleReadQuestionChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ReadQuestion));

    private ReadQuestion _readQuestion;

    public String Name => ReadQuestion.Name;

    public ReadQuestionModel(ReadQuestion readQuestion)
    {
        _readQuestion = readQuestion;
        _readQuestion.PropertyChanged += HandleReadQuestionChanged;
    }

    public void Dispose()
    {
        _readQuestion.PropertyChanged -= HandleReadQuestionChanged;
    }

    public Guid? Id
    {
        get => _readQuestion.Id;
        set => _readQuestion.Id = value;
    }

    public Int32? Ordinal
    {
        get => _readQuestion.Ordinal;
        set => _readQuestion.Ordinal = value;
    }

    public ReadQuestion ReadQuestion
    {
        get => _readQuestion;
        set => SetProperty(ref _readQuestion, value);
    }
}