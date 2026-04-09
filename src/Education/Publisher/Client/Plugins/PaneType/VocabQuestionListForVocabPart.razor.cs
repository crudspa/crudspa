namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class VocabQuestionListForVocabPart : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IVocabQuestionService VocabQuestionService { get; set; } = null!;

    public VocabQuestionListForVocabPartModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, VocabQuestionService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class VocabQuestionListForVocabPartModel : ListOrderablesModel<VocabQuestionModel>,
    IHandle<VocabQuestionAdded>, IHandle<VocabQuestionSaved>, IHandle<VocabQuestionRemoved>, IHandle<VocabQuestionsReordered>
{
    private readonly IVocabQuestionService _vocabQuestionService;
    private readonly Guid? _vocabPartId;

    public VocabQuestionListForVocabPartModel(IEventBus eventBus, IScrollService scrollService, IVocabQuestionService vocabQuestionService, Guid? vocabPartId)
        : base(scrollService)
    {
        _vocabQuestionService = vocabQuestionService;

        _vocabPartId = vocabPartId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(VocabQuestionAdded payload) => await Replace(payload.Id, payload.VocabPartId);

    public async Task Handle(VocabQuestionSaved payload) => await Replace(payload.Id, payload.VocabPartId);

    public async Task Handle(VocabQuestionRemoved payload) => await Rid(payload.Id, payload.VocabPartId);

    public async Task Handle(VocabQuestionsReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<VocabPart>(new() { Id = _vocabPartId });
        var response = await WithWaiting("Fetching...", () => _vocabQuestionService.FetchForVocabPart(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new VocabQuestionModel(x)).ToList());
    }

    public override async Task<Response<VocabQuestionModel?>> Fetch(Guid? id)
    {
        var response = await _vocabQuestionService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new VocabQuestionModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _vocabQuestionService.Remove(new(new()
        {
            Id = id,
            VocabPartId = _vocabPartId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_vocabPartId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.VocabQuestion).ToList();
        return await WithWaiting("Saving...", () => _vocabQuestionService.SaveOrder(new(orderables)));
    }
}

public class VocabQuestionModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleVocabQuestionChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(VocabQuestion));

    private VocabQuestion _vocabQuestion;

    public String? Name => VocabQuestion.Name;

    public VocabQuestionModel(VocabQuestion vocabQuestion)
    {
        _vocabQuestion = vocabQuestion;
        _vocabQuestion.PropertyChanged += HandleVocabQuestionChanged;
    }

    public void Dispose()
    {
        _vocabQuestion.PropertyChanged -= HandleVocabQuestionChanged;
    }

    public Guid? Id
    {
        get => _vocabQuestion.Id;
        set => _vocabQuestion.Id = value;
    }

    public Int32? Ordinal
    {
        get => _vocabQuestion.Ordinal;
        set => _vocabQuestion.Ordinal = value;
    }

    public VocabQuestion VocabQuestion
    {
        get => _vocabQuestion;
        set => SetProperty(ref _vocabQuestion, value);
    }
}