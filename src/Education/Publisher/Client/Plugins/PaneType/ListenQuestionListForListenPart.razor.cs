namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ListenQuestionListForListenPart : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IListenQuestionService ListenQuestionService { get; set; } = null!;

    public ListenQuestionListForListenPartModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ListenQuestionService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ListenQuestionListForListenPartModel : ListOrderablesModel<ListenQuestionModel>,
    IHandle<ListenQuestionAdded>, IHandle<ListenQuestionSaved>, IHandle<ListenQuestionRemoved>, IHandle<ListenQuestionsReordered>
{
    private readonly IListenQuestionService _listenQuestionService;
    private readonly Guid? _listenPartId;

    public ListenQuestionListForListenPartModel(IEventBus eventBus, IScrollService scrollService, IListenQuestionService listenQuestionService, Guid? listenPartId)
        : base(scrollService)
    {
        _listenQuestionService = listenQuestionService;
        _listenPartId = listenPartId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ListenQuestionAdded payload) => await Replace(payload.Id, payload.ListenPartId);

    public async Task Handle(ListenQuestionSaved payload) => await Replace(payload.Id, payload.ListenPartId);

    public async Task Handle(ListenQuestionRemoved payload) => await Rid(payload.Id, payload.ListenPartId);

    public async Task Handle(ListenQuestionsReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<ListenPart>(new() { Id = _listenPartId });
        var response = await WithWaiting("Fetching...", () => _listenQuestionService.FetchForListenPart(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ListenQuestionModel(x)).ToList());
    }

    public override async Task<Response<ListenQuestionModel?>> Fetch(Guid? id)
    {
        var response = await _listenQuestionService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ListenQuestionModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _listenQuestionService.Remove(new(new()
        {
            Id = id,
            ListenPartId = _listenPartId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_listenPartId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.ListenQuestion).ToList();
        return await WithWaiting("Saving...", () => _listenQuestionService.SaveOrder(new(orderables)));
    }
}

public class ListenQuestionModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleListenQuestionChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ListenQuestion));

    private ListenQuestion _listenQuestion;

    public String Name => ListenQuestion.Name;

    public ListenQuestionModel(ListenQuestion listenQuestion)
    {
        _listenQuestion = listenQuestion;
        _listenQuestion.PropertyChanged += HandleListenQuestionChanged;
    }

    public void Dispose()
    {
        _listenQuestion.PropertyChanged -= HandleListenQuestionChanged;
    }

    public Guid? Id
    {
        get => _listenQuestion.Id;
        set => _listenQuestion.Id = value;
    }

    public Int32? Ordinal
    {
        get => _listenQuestion.Ordinal;
        set => _listenQuestion.Ordinal = value;
    }

    public ListenQuestion ListenQuestion
    {
        get => _listenQuestion;
        set => SetProperty(ref _listenQuestion, value);
    }
}