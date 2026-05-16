namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class SurveyDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISurveyRunService SurveyRunService { get; set; } = null!;

    public SurveyDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<SurveyConfig>();

        if (config?.SurveyId.HasSomething() == true)
            Id = config.SurveyId;

        Model = new(Path, Id, EventBus, Navigator, ElementProgressService, ScrollService, SurveyRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SurveyDisplayModel : ScreenModel, IDisposable
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly IEventBus _eventBus;
    private readonly INavigator _navigator;
    private readonly IElementProgressService _elementProgressService;
    private readonly ISurveyRunService _surveyRunService;

    public SurveyDisplayModel(String? path,
        Guid? id,
        IEventBus eventBus,
        INavigator navigator,
        IElementProgressService elementProgressService,
        IScrollService scrollService,
        ISurveyRunService surveyRunService)
    {
        _path = path;
        _id = id;
        _eventBus = eventBus;
        _navigator = navigator;
        _elementProgressService = elementProgressService;
        _surveyRunService = surveyRunService;
        CompletionModalModel = new(scrollService);
    }

    public Survey? Survey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 CurrentPartIndex
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                RaisePropertyChanged(nameof(CurrentPart));
                RaisePropertyChanged(nameof(CanMoveBack));
                RaisePropertyChanged(nameof(CanMoveNext));
                RaisePropertyChanged(nameof(PositionText));
            }
        }
    }

    public ModalModel CompletionModalModel { get; }

    public Dictionary<Guid, QuestionDisplayModel> QuestionModels { get; } = [];

    public SurveyPart? CurrentPart => Survey?.Parts.ElementAtOrDefault(CurrentPartIndex);

    public Boolean CanMoveBack => CurrentPartIndex > 0;

    public Boolean CanMoveNext => Survey?.Parts is { Count: > 0 } parts && CurrentPartIndex < parts.Count - 1;

    public Boolean IsComplete => Survey?.Reply?.Completed.HasValue == true;

    public String PositionText => Survey?.Parts is { Count: > 1 } parts
        ? $"Part {CurrentPartIndex + 1} of {parts.Count}"
        : String.Empty;

    public async Task Refresh()
    {
        if (!_id.HasValue)
        {
            Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Survey is not configured." });
            return;
        }

        var response = await WithWaiting("Loading...", () =>
            _surveyRunService.Fetch(new(new() { Id = _id })));

        if (response.Value is not null)
            await SetSurvey(response.Value);
        else if (response.Errors.IsEmpty())
            Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Survey is not available." });
    }

    public async Task Back()
    {
        if (!CanMoveBack)
            return;

        CurrentPartIndex--;
        await Task.CompletedTask;
    }

    public async Task Next()
    {
        if (!CanMoveNext || !await SaveCurrentPart())
            return;

        CurrentPartIndex++;
    }

    public async Task Finish()
    {
        if (IsComplete)
            return;

        if (!await SaveCurrentPart())
            return;

        if (Survey?.Reply?.Id is not { } replyId)
        {
            Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Sign in to complete this survey." });
            return;
        }

        var response = await WithWaiting("Completing...", () =>
            _surveyRunService.Complete(new(new() { Id = replyId })));

        if (response.Ok)
        {
            Survey.Reply.Completed = DateTimeOffset.Now;
            CurrentPartIndex = 0;
            await CompletionModalModel.Show();
        }
    }

    public override void Dispose()
    {
        foreach (var model in QuestionModels.Values)
            model.Dispose();

        base.Dispose();
    }

    private async Task SetSurvey(Survey survey)
    {
        foreach (var model in QuestionModels.Values)
            model.Dispose();

        QuestionModels.Clear();

        var repliesByQuestionId = survey.Reply?.QuestionReplies
            .Where(x => x.QuestionId.HasValue)
            .GroupBy(x => x.QuestionId!.Value)
            .ToDictionary(x => x.Key,
                x => x.OrderByDescending(reply => reply.Submitted ?? DateTimeOffset.MinValue).First());

        Survey = survey;
        CurrentPartIndex = DetermineCurrentPartIndex(survey, repliesByQuestionId);

        foreach (var surveyQuestion in survey.Parts.SelectMany(x => x.Questions))
        {
            var question = surveyQuestion.Question;
            if (question.Id is not { } questionId)
                continue;

            QuestionReply? reply = null;
            repliesByQuestionId?.TryGetValue(questionId, out reply);
            QuestionModels[questionId] = new(_eventBus, _elementProgressService, question, survey.Reply?.Id, reply);
            await QuestionModels[questionId].Initialize();
        }

        _navigator.UpdateTitle(_path, Survey.Title);
    }

    private static Int32 DetermineCurrentPartIndex(Survey survey, Dictionary<Guid, QuestionReply>? repliesByQuestionId)
    {
        if (survey.Reply?.Completed.HasValue == true || survey.Parts.IsEmpty())
            return 0;

        if (repliesByQuestionId is null)
            return 0;

        if (repliesByQuestionId.IsEmpty())
            return 0;

        for (var index = 0; index < survey.Parts.Count; index++)
        {
            var questions = survey.Parts[index].Questions;

            if (questions.Any(x => x.Question.Id.HasValue && !repliesByQuestionId.ContainsKey(x.Question.Id.Value)))
                return index;
        }

        return survey.Parts.Count - 1;
    }

    private async Task<Boolean> SaveCurrentPart()
    {
        if (CurrentPart is null)
            return true;

        var ok = true;

        foreach (var surveyQuestion in CurrentPart.Questions)
        {
            if (IsComplete)
                return false;

            if (surveyQuestion.Question.Id is not { } questionId || !QuestionModels.TryGetValue(questionId, out var questionModel))
                continue;

            await questionModel.Save();

            if (questionModel.Alerts.Any(x => x.Type == Alert.AlertType.Error))
                ok = false;
        }

        return ok;
    }
}