using Crudspa.Education.Student.Client.Contracts.Events;
using static Crudspa.Framework.Core.Shared.Contracts.Data.Alert;

namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class AssessmentDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IMediaPlayService MediaPlayService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IAssessmentRunService AssessmentRunService { get; set; } = null!;

    public AssessmentDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, ScrollService, SoundService, AssessmentRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleAudioPlayed(MediaPlay mediaPlay)
    {
        await MediaPlayService.Add(new(mediaPlay));
    }
}

public class AssessmentDisplayModel : ScreenModel
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly IEventBus _eventBus;
    private readonly INavigator _navigator;
    private readonly IScrollService _scrollService;
    private readonly ISoundService _soundService;
    private readonly IAssessmentRunService _assessmentRunService;

    public AssessmentDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService,
        ISoundService soundService,
        IAssessmentRunService assessmentRunService)
    {
        _path = path;
        _id = id;
        _eventBus = eventBus;
        _navigator = navigator;
        _scrollService = scrollService;
        _soundService = soundService;
        _assessmentRunService = assessmentRunService;

        eventBus.Subscribe(this);
    }

    public Assessment? Assessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AssessmentStates State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<VocabPartModel> VocabPartModels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public VocabPartModel? VocabPartModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ListenPartModel> ListenPartModels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ListenPartModel? ListenPartModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ReadPartModel> ReadPartModels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ReadPartModel? ReadPartModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 QuestionCount
    {
        get;
        set => SetProperty(ref field, value);
    } = 1;

    public Int32 CurrentQuestion
    {
        get;
        set => SetProperty(ref field, value);
    } = 1;

    public Double CompletionPercentage
    {
        get;
        set => SetProperty(ref field, value);
    } = 1;

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _assessmentRunService.FetchAssessment(new(new() { Id = _id })));

        if (response.Ok)
        {
            await SetAssessment(response.Value);
            await MoveToNextPart();
        }
    }

    private void UpdateProgress(Boolean playAudio = true)
    {
        _eventBus.Publish(new SilenceRequested());

        switch (State)
        {
            case AssessmentStates.VocabPreview:
                CurrentQuestion = VocabPartModel!.PreviewQuestions[0].Number.GetValueOrDefault();
                break;
            case AssessmentStates.VocabQuestions:
                CurrentQuestion = VocabPartModel!.QuestionCollection.Current[0].Number.GetValueOrDefault();
                break;
            case AssessmentStates.ListenPassage:
                CurrentQuestion = VocabPartModel?.QuestionCollection.Current[0].Number.GetValueOrDefault() + 1 ?? 1;
                break;
            case AssessmentStates.ListenPreview:
                CurrentQuestion = ListenPartModel!.PreviewQuestions[0].Number.GetValueOrDefault();
                break;
            case AssessmentStates.ListenQuestions:
                CurrentQuestion = ListenPartModel!.QuestionCollection.Current[0].Number.GetValueOrDefault();
                break;
            case AssessmentStates.ReadPreview:
                CurrentQuestion = ReadPartModel!.PreviewQuestions[0].Number.GetValueOrDefault();
                break;
            case AssessmentStates.ReadQuestions:
                if (ReadPartModel!.QuestionCollection.Current.HasItems())
                    CurrentQuestion = ReadPartModel!.QuestionCollection.Current[0].Number.GetValueOrDefault();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        RaisePropertyChanged(nameof(CurrentQuestion));
        CompletionPercentage = CurrentQuestion / (Double)QuestionCount * 100;
    }

    private async Task MoveToNextPart()
    {
        AssessmentStates? nextState = null;

        foreach (var vocabPart in Assessment!.VocabParts)
        {
            var completed = Assessment.Assignment!.VocabPartCompleteds.FirstOrDefault(x => x.VocabPartId == vocabPart.Id);

            if (completed is null)
            {
                var model = VocabPartModels.First(x => x.VocabPart.Id == vocabPart.Id);

                if (model.PreviewQuestions.HasItems() && model.PreviewQuestions.HasAny(x => x.VocabQuestion.Selections.IsEmpty()))
                    nextState = AssessmentStates.VocabPreview;
                else
                    nextState = AssessmentStates.VocabQuestions;

                VocabPartModel = model;
                break;
            }
        }

        if (!nextState.HasValue)
        {
            foreach (var listenPart in Assessment.ListenParts)
            {
                var completed = Assessment.Assignment!.ListenPartCompleteds.FirstOrDefault(x => x.ListenPartId == listenPart.Id);

                if (completed is null)
                {
                    var model = ListenPartModels.FirstOrDefault(x => x.ListenPart.Id == listenPart.Id);
                    nextState = AssessmentStates.ListenPassage;

                    ListenPartModel = model;
                    break;
                }
            }
        }

        if (!nextState.HasValue)
        {
            foreach (var readPart in Assessment.ReadParts)
            {
                var completed = Assessment.Assignment!.ReadPartCompleteds.FirstOrDefault(x => x.ReadPartId == readPart.Id);
                if (completed is null)
                {
                    var model = ReadPartModels.First(x => x.ReadPart.Id == readPart.Id);

                    if (model.PreviewQuestions.HasItems() && model.PreviewQuestions.HasAny(x => x.ReadQuestion.Selections.IsEmpty()))
                        nextState = AssessmentStates.ReadPreview;
                    else
                        nextState = AssessmentStates.ReadQuestions;

                    ReadPartModel = model;
                    break;
                }
            }
        }

        if (nextState.HasValue)
        {
            State = nextState.Value;
            UpdateProgress();
            await _scrollService.ToTop();
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _assessmentRunService.AddAssessmentCompleted(new(Assessment.Assignment!)));

            if (response.Ok)
            {
                await _eventBus.Publish(new MadeProgress
                {
                    Title = "Awesome Job!",
                    Description = "You earned a certificate for completing the assessment!",
                    ImageUrl = "/api/education/student/images/certificate",
                });
            }

            _navigator.Close();
        }
    }

    public async Task CheckVocabPreview()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        var hasIncorrectNumberOfSelections = false;
        var incorrectAnswers = 0;

        foreach (var questionModel in VocabPartModel!.PreviewQuestions)
        {
            foreach (var choiceModel in questionModel.ChoiceModels)
            {
                if (choiceModel.State == AssessmentChoiceStates.Invalid)
                    choiceModel.State = AssessmentChoiceStates.Default;
            }

            var selectionCount = questionModel.ChoiceModels.Count(x => x.State != AssessmentChoiceStates.Default);

            if (selectionCount != 2)
            {
                hasIncorrectNumberOfSelections = true;
                questionModel.State = AssessmentQuestionStates.Invalid;
            }

            foreach (var choiceModel in questionModel.ChoiceModels)
            {
                if (choiceModel.State != AssessmentChoiceStates.Default)
                {
                    if (choiceModel.VocabChoice.IsCorrect == true)
                        choiceModel.State = AssessmentChoiceStates.Valid;
                    else
                        choiceModel.State = AssessmentChoiceStates.Invalid;
                }
            }

            var invalidCount = questionModel.ChoiceModels.Count(x => x.State == AssessmentChoiceStates.Invalid);

            if (invalidCount > 0)
            {
                incorrectAnswers += invalidCount;
                questionModel.State = AssessmentQuestionStates.Invalid;
            }
        }

        VocabPartModel.State = hasIncorrectNumberOfSelections || incorrectAnswers > 0
            ? AssessmentPartStates.Invalid
            : AssessmentPartStates.Valid;

        if (VocabPartModel!.State == AssessmentPartStates.Valid)
        {
            _soundService.ChoiceCorrect();

            Alerts.Add(new() { Type = AlertType.Success, Message = "Nice! You did it!" });
        }
        else
        {
            _soundService.ChoiceIncorrect();

            if (hasIncorrectNumberOfSelections)
                Alerts.Add(new() { Type = AlertType.Error, Message = "Two words must be selected for each question." });

            if (incorrectAnswers == 1)
                Alerts.Add(new() { Type = AlertType.Error, Message = "1 answer is incorrect." });
            else if (incorrectAnswers > 1)
                Alerts.Add(new() { Type = AlertType.Error, Message = $"{incorrectAnswers} answers are incorrect." });
        }

        await _scrollService.ToBottom();
    }

    public async Task AdvanceVocabPreview()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        if (VocabPartModel!.QuestionCollection.Lists.HasItems())
        {
            State = AssessmentStates.VocabQuestions;
            UpdateProgress();
            await _scrollService.ToTop();
        }
        else
            await AdvanceVocabQuestions();
    }

    public async Task AdvanceVocabQuestions()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        var hasIncorrectNumberOfSelections = false;

        foreach (var questionModel in VocabPartModel!.QuestionCollection.Current)
        {
            var selectionCount = questionModel.ChoiceModels.Count(x => x.State == AssessmentChoiceStates.Selected);

            if (selectionCount != 2)
            {
                hasIncorrectNumberOfSelections = true;
                questionModel.State = AssessmentQuestionStates.Invalid;
            }
            else
                questionModel.State = AssessmentQuestionStates.Valid;
        }

        if (hasIncorrectNumberOfSelections)
        {
            _soundService.ChoiceIncorrect();
            Alerts.Add(new() { Type = AlertType.Error, Message = "Two words must be selected for each question." });
            await _scrollService.ToBottom();
        }
        else
        {
            var index = VocabPartModel.QuestionCollection.Lists.IndexOf(VocabPartModel.QuestionCollection.Current);
            var count = VocabPartModel.QuestionCollection.Lists.Count;

            if (index < count - 1)
            {
                VocabPartModel.QuestionCollection.Current = VocabPartModel.QuestionCollection.Lists[index + 1];
                UpdateProgress(false);
            }
            else
            {
                var completed = new VocabPartCompleted
                {
                    Id = Guid.NewGuid(),
                    AssignmentId = Assessment!.Assignment!.Id,
                    VocabPartId = VocabPartModel.VocabPart.Id,
                    DeviceTimestamp = DateTimeOffset.Now,
                };

                Assessment.Assignment.VocabPartCompleteds.Add(completed);

                var response = await WithWaiting("Saving...", () => _assessmentRunService.AddVocabPartCompleted(new(completed)));

                if (response.Ok)
                    await MoveToNextPart();
            }
        }
    }

    public async Task AdvanceListenPassage()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        if (ListenPartModel!.PreviewQuestions.Any())
        {
            State = AssessmentStates.ListenPreview;
            UpdateProgress();
            await _scrollService.ToTop();
        }
        else if (ListenPartModel!.QuestionCollection.Lists.Any())
        {
            State = AssessmentStates.ListenQuestions;
            UpdateProgress();
            await _scrollService.ToTop();
        }
        else
            await AdvanceListenQuestions();
    }

    public async Task CheckListenPreview()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        var hasIncorrectNumberOfSelections = false;
        var incorrectAnswers = 0;

        foreach (var questionModel in ListenPartModel!.PreviewQuestions)
        {
            var selectionCount = questionModel.ChoiceModels.Count(x => x.State != AssessmentChoiceStates.Default);

            if (selectionCount != 1)
            {
                hasIncorrectNumberOfSelections = true;
                questionModel.State = AssessmentQuestionStates.Invalid;
            }

            foreach (var choiceModel in questionModel.ChoiceModels)
            {
                if (choiceModel.State != AssessmentChoiceStates.Default)
                {
                    choiceModel.State = choiceModel.ListenChoice.IsCorrect == true
                        ? AssessmentChoiceStates.Valid
                        : AssessmentChoiceStates.Invalid;
                }
            }

            var invalidCount = questionModel.ChoiceModels.Count(x => x.State == AssessmentChoiceStates.Invalid);

            if (invalidCount > 0)
            {
                incorrectAnswers += invalidCount;
                questionModel.State = AssessmentQuestionStates.Invalid;
            }
        }

        ListenPartModel.State = hasIncorrectNumberOfSelections || incorrectAnswers > 0
            ? AssessmentPartStates.Invalid
            : AssessmentPartStates.Valid;

        if (ListenPartModel.State == AssessmentPartStates.Valid)
        {
            _soundService.ChoiceCorrect();
            Alerts.Add(new() { Type = AlertType.Success, Message = "Nice! You did it!" });
        }
        else
        {
            _soundService.ChoiceIncorrect();

            if (hasIncorrectNumberOfSelections)
                Alerts.Add(new() { Type = AlertType.Error, Message = "Each question must have an answer." });

            if (incorrectAnswers == 1)
                Alerts.Add(new() { Type = AlertType.Error, Message = "1 answer is incorrect." });
            else if (incorrectAnswers > 1)
                Alerts.Add(new() { Type = AlertType.Error, Message = $"{incorrectAnswers} answers are incorrect." });
        }

        await _scrollService.ToBottom();
    }

    public async Task AdvanceListenPreview()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        if (ListenPartModel!.QuestionCollection.Lists.Any())
        {
            State = AssessmentStates.ListenQuestions;
            UpdateProgress();
            await _scrollService.ToTop();
        }
        else
            await AdvanceListenQuestions();
    }

    public async Task AdvanceListenQuestions()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        var hasIncorrectNumberOfSelections = false;
        var hasMissingText = false;

        foreach (var questionModel in ListenPartModel!.QuestionCollection.Current)
        {
            if (questionModel.ListenQuestion.RequireTextInput == true)
            {
                if (questionModel.TextEntry.HasNothing())
                {
                    hasMissingText = true;
                    questionModel.State = AssessmentQuestionStates.Invalid;
                }
                else
                    questionModel.State = AssessmentQuestionStates.Valid;
            }
            else
            {
                var selectionCount = questionModel.ChoiceModels.Count(x => x.State == AssessmentChoiceStates.Selected);

                if (selectionCount != 1)
                {
                    hasIncorrectNumberOfSelections = true;
                    questionModel.State = AssessmentQuestionStates.Invalid;
                }
                else
                    questionModel.State = AssessmentQuestionStates.Valid;
            }
        }

        if (hasIncorrectNumberOfSelections || hasMissingText)
        {
            _soundService.ChoiceIncorrect();
            Alerts.Add(new() { Type = AlertType.Error, Message = "Each question must have an answer." });
            await _scrollService.ToBottom();
        }
        else
        {
            var index = ListenPartModel.QuestionCollection.Lists.IndexOf(ListenPartModel.QuestionCollection.Current);
            var count = ListenPartModel.QuestionCollection.Lists.Count;

            if (index < count - 1)
            {
                ListenPartModel.QuestionCollection.Current = ListenPartModel.QuestionCollection.Lists[index + 1];
                UpdateProgress(false);
            }
            else
            {
                var completed = new ListenPartCompleted
                {
                    Id = Guid.NewGuid(),
                    AssignmentId = Assessment!.Assignment!.Id,
                    ListenPartId = ListenPartModel.ListenPart.Id,
                    DeviceTimestamp = DateTimeOffset.Now,
                };

                Assessment.Assignment.ListenPartCompleteds.Add(completed);

                var response = await WithWaiting("Saving...", () => _assessmentRunService.AddListenPartCompleted(new(completed)));

                if (response.Ok)
                    await MoveToNextPart();
            }
        }
    }

    public async Task CheckReadPreview()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        var hasIncorrectNumberOfSelections = false;
        var incorrectAnswers = 0;

        foreach (var questionModel in ReadPartModel!.PreviewQuestions)
        {
            var selectionCount = questionModel.ChoiceModels.Count(x => x.State != AssessmentChoiceStates.Default);

            if (selectionCount != 1)
            {
                hasIncorrectNumberOfSelections = true;
                questionModel.State = AssessmentQuestionStates.Invalid;
            }

            foreach (var choiceModel in questionModel.ChoiceModels)
            {
                if (choiceModel.State != AssessmentChoiceStates.Default)
                {
                    choiceModel.State = choiceModel.ReadChoice.IsCorrect == true
                        ? AssessmentChoiceStates.Valid
                        : AssessmentChoiceStates.Invalid;
                }
            }

            var invalidCount = questionModel.ChoiceModels.Count(x => x.State == AssessmentChoiceStates.Invalid);

            if (invalidCount > 0)
            {
                incorrectAnswers += invalidCount;
                questionModel.State = AssessmentQuestionStates.Invalid;
            }
        }

        ReadPartModel.State = hasIncorrectNumberOfSelections || incorrectAnswers > 0
            ? AssessmentPartStates.Invalid
            : AssessmentPartStates.Valid;

        if (ReadPartModel.State == AssessmentPartStates.Valid)
        {
            _soundService.ChoiceCorrect();
            Alerts.Add(new() { Type = AlertType.Success, Message = "Nice! You did it!" });
        }
        else
        {
            _soundService.ChoiceIncorrect();

            if (hasIncorrectNumberOfSelections)
                Alerts.Add(new() { Type = AlertType.Error, Message = "Each question must have an answer." });

            if (incorrectAnswers == 1)
                Alerts.Add(new() { Type = AlertType.Error, Message = "1 answer is incorrect." });
            else if (incorrectAnswers > 1)
                Alerts.Add(new() { Type = AlertType.Error, Message = $"{incorrectAnswers} answers are incorrect." });
        }

        await _scrollService.ToBottom();
    }

    public async Task AdvanceReadPreview()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        if (ReadPartModel!.QuestionCollection.Lists.Any())
        {
            State = AssessmentStates.ReadQuestions;
            UpdateProgress();
            await _scrollService.ToTop();
        }
        else
            await AdvanceReadQuestions();
    }

    public async Task AdvanceReadQuestions()
    {
        _soundService.ButtonPress();
        Alerts.Clear();

        var hasIncorrectNumberOfSelections = false;
        var hasMissingText = false;

        foreach (var questionModel in ReadPartModel!.QuestionCollection.Current)
        {
            if (questionModel.ReadQuestion.RequireTextInput == true)
            {
                if (questionModel.TextEntry.HasNothing())
                {
                    hasMissingText = true;
                    questionModel.State = AssessmentQuestionStates.Invalid;
                }
                else
                    questionModel.State = AssessmentQuestionStates.Valid;
            }
            else
            {
                var selectionCount = questionModel.ChoiceModels.Count(x => x.State == AssessmentChoiceStates.Selected);

                if (selectionCount != 1)
                {
                    hasIncorrectNumberOfSelections = true;
                    questionModel.State = AssessmentQuestionStates.Invalid;
                }
                else
                    questionModel.State = AssessmentQuestionStates.Valid;
            }
        }

        if (hasIncorrectNumberOfSelections || hasMissingText)
        {
            _soundService.ChoiceIncorrect();
            Alerts.Add(new() { Type = AlertType.Error, Message = "Each question must have an answer." });
            await _scrollService.ToBottom();
        }
        else
        {
            var index = ReadPartModel.QuestionCollection.Lists.IndexOf(ReadPartModel.QuestionCollection.Current);
            var count = ReadPartModel.QuestionCollection.Lists.Count;

            if (index < count - 1)
            {
                ReadPartModel.QuestionCollection.Current = ReadPartModel.QuestionCollection.Lists[index + 1];
                UpdateProgress(false);
            }
            else
            {
                var completed = new ReadPartCompleted
                {
                    Id = Guid.NewGuid(),
                    AssignmentId = Assessment!.Assignment!.Id,
                    ReadPartId = ReadPartModel.ReadPart.Id,
                    DeviceTimestamp = DateTimeOffset.Now,
                };

                Assessment.Assignment.ReadPartCompleteds.Add(completed);

                var response = await WithWaiting("Saving...", () => _assessmentRunService.AddReadPartCompleted(new(completed)));

                if (response.Ok)
                    await MoveToNextPart();
            }
        }
    }

    private void AssignQuestionNumbers()
    {
        var questionNumber = 1;

        foreach (var partModel in VocabPartModels)
        {
            foreach (var questionModel in partModel.PreviewQuestions)
            {
                questionModel.Number = questionNumber;
                questionNumber++;
            }

            foreach (var list in partModel.QuestionCollection.Lists)
            {
                foreach (var questionModel in list)
                {
                    questionModel.Number = questionNumber;
                    questionNumber++;
                }
            }
        }

        foreach (var partModel in ListenPartModels)
        {
            foreach (var questionModel in partModel.PreviewQuestions)
            {
                questionModel.Number = questionNumber;
                questionNumber++;
            }

            foreach (var list in partModel.QuestionCollection.Lists)
            {
                foreach (var questionModel in list)
                {
                    questionModel.Number = questionNumber;
                    questionNumber++;
                }
            }
        }

        foreach (var partModel in ReadPartModels)
        {
            foreach (var questionModel in partModel.PreviewQuestions)
            {
                questionModel.Number = questionNumber;
                questionNumber++;
            }

            foreach (var list in partModel.QuestionCollection.Lists)
            {
                foreach (var questionModel in list)
                {
                    questionModel.Number = questionNumber;
                    questionNumber++;
                }
            }
        }

        QuestionCount = questionNumber;
    }

    private Task SetAssessment(Assessment assessment)
    {
        Assessment = assessment;
        _navigator.UpdateTitle(_path, Assessment.Name!);

        VocabPartModels = assessment.VocabParts.Select(x => new VocabPartModel(_assessmentRunService, x, assessment.Assignment!.Id)).ToObservable();
        ListenPartModels = assessment.ListenParts.Select(x => new ListenPartModel(_assessmentRunService, x, assessment.Assignment!.Id)).ToObservable();
        ReadPartModels = assessment.ReadParts.Select(x => new ReadPartModel(_assessmentRunService, x, assessment.Assignment!.Id)).ToObservable();

        AssignQuestionNumbers();

        return Task.CompletedTask;
    }
}

public class ListenPartModel : Observable
{
    public ListenPartModel(IAssessmentRunService assessmentRunService, ListenPart readPart, Guid? assignmentId)
    {
        ListenPart = readPart;

        State = AssessmentPartStates.Accepting;

        PreviewQuestions.AddRange(ListenPart.ListenQuestions
            .Where(x => x.IsPreview == true)
            .Select(x => new ListenQuestionModel(assessmentRunService, assignmentId, x)));

        // Break up the actual questions into multiple lists based on the PageBreakBefore attribute
        var actualQuestions = ListenPart.ListenQuestions.Where(x => x.IsPreview == false);

        if (actualQuestions.Any())
        {
            QuestionCollection = new();
            List<ListenQuestionModel>? currentList = null;

            foreach (var question in actualQuestions)
            {
                if (currentList is null || question.PageBreakBefore == true)
                {
                    currentList = [];
                    QuestionCollection.Lists.Add(currentList);
                }

                currentList.Add(new(assessmentRunService, assignmentId, question));
            }

            var index = FindFirstWithUnansweredQuestions(QuestionCollection);
            QuestionCollection.Current = QuestionCollection.Lists[index];
        }
    }

    public ListenPart ListenPart { get; }

    public AssessmentPartStates State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ListenQuestionModel> PreviewQuestions
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ListCollection<ListenQuestionModel> QuestionCollection
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    private static Int32 FindFirstWithUnansweredQuestions(ListCollection<ListenQuestionModel> collection)
    {
        for (var i = 0; i < collection.Lists.Count; i++)
            foreach (var questionModel in collection.Lists[i])
                if (questionModel.ListenQuestion.Selections.IsEmpty())
                    return i;

        return collection.Lists.Count - 1;
    }
}

public class ListenQuestionModel : Observable
{
    public ListenQuestionModel(IAssessmentRunService assessmentRunService, Guid? assignmentId, ListenQuestion listenQuestion)
    {
        ListenQuestion = listenQuestion;
        State = AssessmentQuestionStates.Valid;
        ChoiceModels.AddRange(listenQuestion.ListenChoices.Select(x => new ListenChoiceModel(assessmentRunService, assignmentId, x)));
    }

    public ListenQuestion ListenQuestion { get; }

    public AssessmentQuestionStates State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ListenChoiceModel> ChoiceModels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String? TextEntry
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Number
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task SelectChoice(Guid? id)
    {
        foreach (var choiceModel in ChoiceModels)
        {
            if (choiceModel.ListenChoice.Id.Equals(id))
            {
                if (choiceModel.State == AssessmentChoiceStates.Default)
                    await choiceModel.Select();
                else
                    choiceModel.Unselect();
            }
            else
                choiceModel.Unselect();
        }

        if (State == AssessmentQuestionStates.Invalid)
        {
            if (ListenQuestion.RequireTextInput == true)
                if (TextEntry.HasSomething())
                    State = AssessmentQuestionStates.Valid;
        }
        else
        {
            var selectionCount = ChoiceModels.Count(x => x.State != AssessmentChoiceStates.Default);

            if (selectionCount == 1)
                State = AssessmentQuestionStates.Valid;
        }
    }

    public void HandleTextChanged()
    {
        if (State == AssessmentQuestionStates.Invalid && TextEntry.HasSomething())
            State = AssessmentQuestionStates.Valid;
    }
}

public class ListenChoiceModel : AssessmentChoiceModelBase
{
    private readonly IAssessmentRunService _assessmentRunService;
    private readonly Guid? _assignmentId;

    public ListenChoiceModel(IAssessmentRunService assessmentRunService, Guid? assignmentId, ListenChoice readChoice)
    {
        _assessmentRunService = assessmentRunService;
        _assignmentId = assignmentId;

        ListenChoice = readChoice;
        State = AssessmentChoiceStates.Default;
    }

    public ListenChoice ListenChoice { get; }

    public async Task Select()
    {
        State = AssessmentChoiceStates.Selected;

        var selection = new ListenChoiceSelection
        {
            AssignmentId = _assignmentId,
            ChoiceId = ListenChoice.Id,
        };

        var response = await _assessmentRunService.AddListenChoiceSelection(new(selection));

        if (!response.Ok)
            Unselect();
    }

    public void Unselect()
    {
        State = AssessmentChoiceStates.Default;
    }
}

public class ReadPartModel : Observable
{
    public ReadPartModel(IAssessmentRunService assessmentRunService, ReadPart readPart, Guid? assignmentId)
    {
        ReadPart = readPart;

        State = AssessmentPartStates.Accepting;

        PreviewQuestions.AddRange(ReadPart.ReadQuestions
            .Where(x => x.IsPreview == true)
            .Select(x => new ReadQuestionModel(assessmentRunService, assignmentId, x)));

        // Break up the actual questions into multiple lists based on the PageBreakBefore attribute
        var actualQuestions = ReadPart.ReadQuestions.Where(x => x.IsPreview == false);

        if (actualQuestions.Any())
        {
            QuestionCollection = new();
            List<ReadQuestionModel>? currentList = null;

            foreach (var question in actualQuestions)
            {
                if (currentList is null || question.PageBreakBefore == true)
                {
                    currentList = [];
                    QuestionCollection.Lists.Add(currentList);
                }

                currentList.Add(new(assessmentRunService, assignmentId, question));
            }

            var index = FindFirstWithUnansweredQuestions(QuestionCollection);
            QuestionCollection.Current = QuestionCollection.Lists[index];
        }
    }

    public ReadPart ReadPart { get; }

    public AssessmentPartStates State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ReadQuestionModel> PreviewQuestions
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ListCollection<ReadQuestionModel> QuestionCollection
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    private static Int32 FindFirstWithUnansweredQuestions(ListCollection<ReadQuestionModel> collection)
    {
        for (var i = 0; i < collection.Lists.Count; i++)
            foreach (var questionModel in collection.Lists[i])
                if (questionModel.ReadQuestion.Selections.IsEmpty())
                    return i;

        return collection.Lists.Count - 1;
    }
}

public class ReadQuestionModel : Observable
{
    public ReadQuestionModel(IAssessmentRunService assessmentRunService, Guid? assignmentId, ReadQuestion readQuestion)
    {
        ReadQuestion = readQuestion;
        State = AssessmentQuestionStates.Valid;
        ChoiceModels.AddRange(readQuestion.ReadChoices.Select(x => new ReadChoiceModel(assessmentRunService, assignmentId, x)));
    }

    public ReadQuestion ReadQuestion { get; }

    public AssessmentQuestionStates State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ReadChoiceModel> ChoiceModels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String? TextEntry
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Number
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task SelectChoice(Guid? id)
    {
        foreach (var choiceModel in ChoiceModels)
        {
            if (choiceModel.ReadChoice.Id.Equals(id))
            {
                if (choiceModel.State == AssessmentChoiceStates.Default)
                    await choiceModel.Select();
                else
                    choiceModel.Unselect();
            }
            else
                choiceModel.Unselect();
        }

        if (State == AssessmentQuestionStates.Invalid)
        {
            if (ReadQuestion.RequireTextInput == true)
                if (TextEntry.HasSomething())
                    State = AssessmentQuestionStates.Valid;
        }
        else
        {
            var selectionCount = ChoiceModels.Count(x => x.State != AssessmentChoiceStates.Default);

            if (selectionCount == 1)
                State = AssessmentQuestionStates.Valid;
        }
    }

    public void HandleTextChanged()
    {
        if (State == AssessmentQuestionStates.Invalid && TextEntry.HasSomething())
            State = AssessmentQuestionStates.Valid;
    }
}

public class ReadChoiceModel : AssessmentChoiceModelBase
{
    private readonly IAssessmentRunService _assessmentRunService;
    private readonly Guid? _assignmentId;

    public ReadChoiceModel(IAssessmentRunService assessmentRunService, Guid? assignmentId, ReadChoice readChoice)
    {
        _assessmentRunService = assessmentRunService;
        _assignmentId = assignmentId;

        ReadChoice = readChoice;
        State = AssessmentChoiceStates.Default;
    }

    public ReadChoice ReadChoice { get; }

    public async Task Select()
    {
        State = AssessmentChoiceStates.Selected;

        var selection = new ReadChoiceSelection
        {
            AssignmentId = _assignmentId,
            ChoiceId = ReadChoice.Id,
        };

        var response = await _assessmentRunService.AddReadChoiceSelection(new(selection));

        if (!response.Ok)
            Unselect();
    }

    public void Unselect()
    {
        State = AssessmentChoiceStates.Default;
    }
}

public class VocabPartModel : Observable
{
    public VocabPartModel(IAssessmentRunService assessmentRunService, VocabPart vocabPart, Guid? assignmentId)
    {
        VocabPart = vocabPart;

        State = AssessmentPartStates.Accepting;

        PreviewQuestions.AddRange(VocabPart.VocabQuestions
            .Where(x => x.IsPreview == true)
            .Select(x => new VocabQuestionModel(assessmentRunService, assignmentId, x)));

        // Break up the actual questions into multiple lists based on the PageBreakBefore attribute
        var actualQuestions = VocabPart.VocabQuestions.Where(x => x.IsPreview == false);

        if (actualQuestions.Any())
        {
            QuestionCollection = new();
            List<VocabQuestionModel>? currentList = null;

            foreach (var question in actualQuestions)
            {
                if (currentList is null || question.PageBreakBefore == true)
                {
                    currentList = [];
                    QuestionCollection.Lists.Add(currentList);
                }

                currentList.Add(new(assessmentRunService, assignmentId, question));
            }

            var index = FindFirstWithUnansweredQuestions(QuestionCollection);
            QuestionCollection.Current = QuestionCollection.Lists[index];
        }
    }

    public VocabPart VocabPart { get; }

    public AssessmentPartStates State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<VocabQuestionModel> PreviewQuestions
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ListCollection<VocabQuestionModel> QuestionCollection
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    private static Int32 FindFirstWithUnansweredQuestions(ListCollection<VocabQuestionModel> collection)
    {
        for (var i = 0; i < collection.Lists.Count; i++)
            foreach (var questionModel in collection.Lists[i])
                if (questionModel.VocabQuestion.Selections.IsEmpty())
                    return i;

        return collection.Lists.Count - 1;
    }
}

public class VocabQuestionModel : Observable
{
    private readonly IAssessmentRunService _assessmentRunService;
    private readonly Guid? _assignmentId;

    public VocabQuestionModel(IAssessmentRunService assessmentRunService, Guid? assignmentId, VocabQuestion vocabQuestion)
    {
        _assessmentRunService = assessmentRunService;
        _assignmentId = assignmentId;
        VocabQuestion = vocabQuestion;

        State = AssessmentQuestionStates.Valid;
        ChoiceModels.AddRange(vocabQuestion.VocabChoices.Select(x => new VocabChoiceModel(x)));
    }

    public VocabQuestion VocabQuestion { get; }

    public AssessmentQuestionStates State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<VocabChoiceModel> ChoiceModels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? Number
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task SelectChoice(Guid? id)
    {
        var choiceModel = ChoiceModels.First(x => x.VocabChoice.Id.Equals(id));

        if (choiceModel.State == default)
            await choiceModel.Select();
        else
            choiceModel.Unselect();

        var choices = ChoiceModels.Where(x => x.State == AssessmentChoiceStates.Selected).Select(x => x.VocabChoice.Id);

        var selection = new VocabChoiceSelection
        {
            AssignmentId = _assignmentId,
            Choices = choices.ToList(),
        };

        var response = await _assessmentRunService.AddVocabChoiceSelection(new(selection));

        if (!response.Ok)
            choiceModel.Unselect();

        if (State == AssessmentQuestionStates.Invalid)
        {
            var selectionCount = ChoiceModels.Count(x => x.State != AssessmentChoiceStates.Default);
            State = selectionCount == 2 ? AssessmentQuestionStates.Valid : AssessmentQuestionStates.Invalid;
        }
    }
}

public class VocabChoiceModel : AssessmentChoiceModelBase
{
    public VocabChoiceModel(VocabChoice vocabChoice)
    {
        VocabChoice = vocabChoice;
        State = AssessmentChoiceStates.Default;
    }

    public VocabChoice VocabChoice { get; }

    public Task Select()
    {
        State = AssessmentChoiceStates.Selected;
        return Task.CompletedTask;
    }

    public void Unselect()
    {
        State = AssessmentChoiceStates.Default;
    }
}

public class AssessmentChoiceModelBase : Observable
{
    public AssessmentChoiceStates State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String StateClass
    {
        get
        {
            switch (State)
            {
                case AssessmentChoiceStates.Selected:
                    return "selected";
                case AssessmentChoiceStates.Invalid:
                    return "invalid";
                case AssessmentChoiceStates.Valid:
                    return "valid";
                case AssessmentChoiceStates.Default:
                default:
                    return "default";
            }
        }
    }
}