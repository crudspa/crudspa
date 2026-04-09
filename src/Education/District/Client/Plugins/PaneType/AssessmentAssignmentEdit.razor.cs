namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class AssessmentAssignmentEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IAssessmentAssignmentService AssessmentAssignmentService { get; set; } = null!;

    public AssessmentAssignmentEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, SessionState, AssessmentAssignmentService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class AssessmentAssignmentEditModel : EditModel<AssessmentAssignment>, IHandle<AssessmentAssignmentSaved>, IHandle<AssessmentAssignmentRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ISessionState _sessionState;
    private readonly IAssessmentAssignmentService _assessmentAssignmentService;

    public AssessmentAssignmentEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        ISessionState sessionState,
        IAssessmentAssignmentService assessmentAssignmentService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _sessionState = sessionState;
        _assessmentAssignmentService = assessmentAssignmentService;

        ProgressModel = new(assessmentAssignmentService, id);

        eventBus.Subscribe(this);
    }

    public AssessmentProgressModel ProgressModel { get; }

    public async Task Handle(AssessmentAssignmentSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(AssessmentAssignmentRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public ObservableCollection<Named> StudentNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> AssessmentNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchStudentNames(),
            FetchAssessmentNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var assignment = new AssessmentAssignment
            {
                StudentId = StudentNames.MinBy(x => x.Name)?.Id,
                AssessmentId = AssessmentNames.MinBy(x => x.Name)?.Id,
            };

            assignment.SetDefaultDates(_sessionState.TimeZoneId);

            SetAssessmentAssignment(assignment);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _assessmentAssignmentService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetAssessmentAssignment(response.Value);

            await ProgressModel.Refresh();
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _assessmentAssignmentService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/assessment-assignment-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _assessmentAssignmentService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchStudentNames()
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchStudentNames(new()), false);
        if (response.Ok) StudentNames = response.Value.ToObservable();
    }

    public async Task FetchAssessmentNames()
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchAssessmentNames(new()), false);
        if (response.Ok) AssessmentNames = response.Value.ToObservable();
    }

    private void SetAssessmentAssignment(AssessmentAssignment assessmentAssignment)
    {
        Entity = assessmentAssignment;
        _navigator.UpdateTitle(_path, IsNew ? "New Assignment" : Entity.Name ?? "Assignment");
    }
}

public class AssessmentProgressModel(IAssessmentAssignmentService assessmentAssignmentService, Guid? assessmentAssignmentId) : ScreenModel
{
    public Assessment? Assessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Fetching...", () => assessmentAssignmentService.FetchProgress(new(new() { Id = assessmentAssignmentId })));

        if (response.Ok)
            await SetAssessment(response.Value);
    }

    private Task SetAssessment(Assessment assessment)
    {
        Assessment = assessment;

        var questionNumber = 1;

        foreach (var part in Assessment!.VocabParts)
        {
            foreach (var question in part.VocabQuestions)
            {
                question.Number = questionNumber++;

                foreach (var choice in question.VocabChoices)
                    if (question.Selections.Any(x => x.ChoiceId == choice.Id))
                        choice.State = choice.IsCorrect == true ? AssessmentChoiceStates.Valid : AssessmentChoiceStates.Invalid;
            }
        }

        foreach (var part in Assessment!.ListenParts)
        {
            foreach (var question in part.ListenQuestions)
            {
                question.Number = questionNumber++;

                foreach (var choice in question.ListenChoices)
                    if (question.Selections.Any(x => x.ChoiceId == choice.Id))
                        choice.State = question.HasCorrectChoice == true
                            ? choice.IsCorrect == true ? AssessmentChoiceStates.Valid : AssessmentChoiceStates.Invalid
                            : AssessmentChoiceStates.Selected;
            }
        }

        foreach (var part in Assessment!.ReadParts)
        {
            foreach (var question in part.ReadQuestions)
            {
                question.Number = questionNumber++;

                foreach (var choice in question.ReadChoices)
                    if (question.Selections.Any(x => x.ChoiceId == choice.Id))
                        choice.State = question.HasCorrectChoice == true
                            ? choice.IsCorrect == true ? AssessmentChoiceStates.Valid : AssessmentChoiceStates.Invalid
                            : AssessmentChoiceStates.Selected;
            }
        }

        return Task.CompletedTask;
    }
}