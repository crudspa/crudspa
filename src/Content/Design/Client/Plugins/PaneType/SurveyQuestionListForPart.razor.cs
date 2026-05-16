namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SurveyQuestionListForPart : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISurveyService SurveyService { get; set; } = null!;

    public SurveyQuestionListForPartModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ElementProgressService, ScrollService, SurveyService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SurveyQuestionListForPartModel : ListOrderablesModel<SurveyQuestionModel>,
    IHandle<SurveyQuestionAdded>, IHandle<SurveyQuestionSaved>, IHandle<SurveyQuestionRemoved>, IHandle<SurveyQuestionsReordered>
{
    private readonly IEventBus _eventBus;
    private readonly IElementProgressService _elementProgressService;
    private readonly ISurveyService _surveyService;
    private readonly Guid? _partId;

    public SurveyQuestionListForPartModel(IEventBus eventBus,
        IElementProgressService elementProgressService,
        IScrollService scrollService,
        ISurveyService surveyService,
        Guid? partId)
        : base(scrollService)
    {
        _eventBus = eventBus;
        _elementProgressService = elementProgressService;
        _surveyService = surveyService;
        _partId = partId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SurveyQuestionAdded payload) => await Replace(payload.Id, payload.PartId);

    public async Task Handle(SurveyQuestionSaved payload) => await Replace(payload.Id, payload.PartId);

    public async Task Handle(SurveyQuestionRemoved payload) => await Rid(payload.Id, payload.PartId);

    public async Task Handle(SurveyQuestionsReordered payload) => await Refresh(false);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var response = await WithWaiting("Fetching...", () =>
            _surveyService.FetchQuestions(new(new() { Id = _partId })), resetAlerts);

        if (response.Ok)
        {
            var questions = response.Value.Select(CreateQuestionModel).ToList();
            await Task.WhenAll(questions.Select(x => x.Initialize()));
            SetCards(questions);
        }
    }

    public override async Task<Response<SurveyQuestionModel?>> Fetch(Guid? id)
    {
        var response = await _surveyService.FetchQuestion(new(new() { Id = id }));

        if (!response.Ok)
            return new() { Errors = response.Errors };

        var question = CreateQuestionModel(response.Value);
        await question.Initialize();
        return new(question);
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _surveyService.RemoveQuestion(new(new()
        {
            Id = id,
            PartId = _partId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_partId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Question).ToList();
        return await WithWaiting("Saving...", () => _surveyService.SaveQuestionOrder(new(orderables)));
    }

    private SurveyQuestionModel CreateQuestionModel(SurveyQuestion question) =>
        new(_eventBus, _elementProgressService, question);
}

public class SurveyQuestionModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleQuestionChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Question));

    private SurveyQuestion _question;

    public SurveyQuestionModel(IEventBus eventBus, IElementProgressService elementProgressService, SurveyQuestion question)
    {
        _question = question;
        _question.PropertyChanged += HandleQuestionChanged;
        Preview = new(eventBus, elementProgressService, _question.Question, null);
    }

    public String? Name => Question.Question.Text.HasSomething() ? Question.Question.Text : "Question";

    public Guid? Id
    {
        get => _question.Id;
        set => _question.Id = value;
    }

    public Int32? Ordinal
    {
        get => _question.Ordinal;
        set => _question.Ordinal = value;
    }

    public SurveyQuestion Question
    {
        get => _question;
        set => SetProperty(ref _question, value);
    }

    public QuestionDisplayModel Preview { get; }

    public async Task Initialize() => await Preview.Initialize();

    public void Dispose()
    {
        _question.PropertyChanged -= HandleQuestionChanged;
        Preview.Dispose();
    }
}