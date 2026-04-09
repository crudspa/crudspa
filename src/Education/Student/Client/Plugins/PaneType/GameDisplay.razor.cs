namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class GameDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public IGameProgressService GameProgressService { get; set; } = null!;
    [Inject] public IActivityRunService ActivityRunService { get; set; } = null!;
    [Inject] public IMediaPlayService MediaPlayService { get; set; } = null!;
    [Inject] public ILogger<GameDisplay> Logger { get; set; } = null!;

    public GameDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, ScrollService, SoundService, StudentAppService, GameProgressService, ActivityRunService, Logger);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleIntroAudioPlayed(MediaPlay mediaPlay)
    {
        _ = await MediaPlayService.Add(new(mediaPlay));
    }
}

public class GameDisplayModel : ScreenModel
{
    private const Int32 MinAccomplishmentSize = 2;
    private const Int32 MaxAccomplishmentSize = 24;

    private async void HandleActivityCompleted(Object? sender, Guid status)
    {
        try
        {
            await MarkAsFinished(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in event handler.");
        }
    }

    private void HandleSurveyModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(SurveyModel));

    public enum States
    {
        Intro,
        Activity,
        Correct,
        Incorrect,
        Survey,
    }

    public AudioFile IntroAudioFile = new() { Id = new Guid("a4b02b76-ea85-43b6-8d0f-615fcabb0578") };
    public SurveyModel SurveyModel { get; } = new();

    private readonly String? _path;
    private readonly Guid? _id;
    private readonly IEventBus _eventBus;
    private readonly INavigator _navigator;
    private readonly IScrollService _scrollService;
    private readonly ISoundService _soundService;
    private readonly IStudentAppService _studentAppService;
    private readonly IGameProgressService _gameProgressService;
    private readonly IActivityRunService _activityRunService;
    private readonly ILogger _logger;

    public GameDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService,
        ISoundService soundService,
        IStudentAppService studentAppService,
        IGameProgressService gameProgressService,
        IActivityRunService activityRunService,
        ILogger logger)
    {
        _path = path;
        _id = id;
        _eventBus = eventBus;
        _navigator = navigator;
        _scrollService = scrollService;
        _soundService = soundService;
        _studentAppService = studentAppService;
        _gameProgressService = gameProgressService;
        _activityRunService = activityRunService;
        _logger = logger;

        SurveyModel.PropertyChanged += HandleSurveyModelChanged;

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        if (CurrentGameActivityModel is not null)
            CurrentGameActivityModel.ActivityCompleted -= HandleActivityCompleted;

        SurveyModel.PropertyChanged -= HandleSurveyModelChanged;

        base.Dispose();
    }

    public Game? Game
    {
        get;
        set => SetProperty(ref field, value);
    }

    public States State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<GameActivityModel> GameActivityModels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public GameActivityModel? CurrentGameActivityModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 ActivityCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 CurrentPosition
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Double AccomplishmentSize
    {
        get;
        set => SetProperty(ref field, value);
    } = 4;

    public Double CompletionPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String BrainStyle
    {
        get
        {
            var scale = Math.Clamp(AccomplishmentSize / MaxAccomplishmentSize, MinAccomplishmentSize / (Double)MaxAccomplishmentSize, 1D);
            return FormattableString.Invariant($"--ces-game-brain-scale: {scale:0.####};");
        }
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchGame(new(new() { Id = _id })));

        if (response.Ok)
        {
            var game = response.Value;

            ActivityCount = game.GameActivities.Count;
            CurrentPosition = 1;

            SetGame(response.Value);

            await MoveToFirst();
        }
    }

    public async Task MoveToFirst()
    {
        var firstUndone = Game!.GameActivities.FirstOrDefault(x =>
            x.Activity!.Assignment.StatusId != ActivityAssignmentStatusIds.Successful
            && x.Activity.Assignment.StatusId != ActivityAssignmentStatusIds.Failed);

        var startIndex = firstUndone is null ? -1 : Game!.GameActivities.IndexOf(firstUndone);

        if (startIndex == 0)
        {
            State = States.Intro;
            _soundService.AchievementGrow();
        }
        else if (startIndex < 0)
            State = States.Survey;
        else
            await MoveTo(startIndex);

        await Task.Yield();
        CalculateAccomplishment();
    }

    public async Task MoveTo(Int32 index)
    {
        CurrentPosition = index + 1;
        CompletionPercentage = CurrentPosition / (Double)ActivityCount * 100;

        if (CurrentGameActivityModel is not null)
            CurrentGameActivityModel.ActivityCompleted -= HandleActivityCompleted;

        CurrentGameActivityModel = GameActivityModels[index];
        CurrentGameActivityModel.ActivityCompleted += HandleActivityCompleted;

        State = States.Activity;
        await MarkAsStarted();
    }

    public async Task MoveNext()
    {
        if (CurrentPosition < ActivityCount)
            await MoveTo(CurrentPosition);
        else
            State = States.Survey;
    }

    public async Task Start()
    {
        await _eventBus.Publish(new SilenceRequested());
        _soundService.ButtonPress();
        await MoveTo(0);
    }

    public async Task MarkAsStarted()
    {
        CurrentGameActivityModel!.GameActivity.Activity!.Assignment.StatusId = ActivityAssignmentStatusIds.Started;
        CurrentGameActivityModel!.GameActivity.Activity.Assignment.Started = DateTimeOffset.Now;

        await SaveStatus();
    }

    public async Task MarkAsFinished(Guid status)
    {
        if (status == ActivityAssignmentStatusIds.Successful)
        {
            _soundService.AchievementGrow();
            State = States.Correct;
        }
        else
        {
            _soundService.AchievementShrink();
            State = States.Incorrect;
        }

        CurrentGameActivityModel!.GameActivity.Activity!.Assignment.StatusId = status;
        CurrentGameActivityModel!.GameActivity.Activity.Assignment.Finished = DateTimeOffset.Now;

        await SaveStatus();

        await Task.Yield();
        CalculateAccomplishment();
    }

    public async Task SaveStatus()
    {
        await WithWaiting("Saving...", () => _activityRunService.SaveActivityState(new(CurrentGameActivityModel!.GameActivity.Activity!.Assignment)));
    }

    public async Task RecordSurveySelection(Guid choiceId)
    {
        _soundService.ChoiceSelected();

        var surveyResponse = new AppSurveyResponse
        {
            AssignmentBatchId = Game!.GameRunId,
            QuestionId = SurveyModel.CurrentStep.Id,
            AnswerId = choiceId,
        };

        var response = await WithWaiting("Saving...", () => _studentAppService.AddSurveyResponse(new(surveyResponse)));

        if (response.Ok)
        {
            var index = SurveyModel.Survey.Steps.IndexOf(SurveyModel.CurrentStep);

            if (index >= SurveyModel.Survey.Steps.Count - 1)
            {
                var gameCompleted = new GameCompleted
                {
                    GameId = Game.Id,
                    DeviceTimestamp = DateTimeOffset.Now,
                    GameRunId = Game.GameRunId,
                };

                await _gameProgressService.AddCompleted(new(gameCompleted));

                _navigator.Close(_path);
            }
            else
            {
                SurveyModel.CurrentStep = SurveyModel.Survey.Steps[index + 1];
                await _scrollService.ToTop();
            }
        }
    }

    private void CalculateAccomplishment()
    {
        if (Game?.GameActivities.Count is not > 0)
            return;

        const Int32 range = MaxAccomplishmentSize - MinAccomplishmentSize;
        var step = range / (Double)Game!.GameActivities.Count / 2;

        var correct = Game.GameActivities.Count(x => x.Activity!.Assignment.StatusId == ActivityAssignmentStatusIds.Successful);
        var incorrect = Game.GameActivities.Count(x => x.Activity!.Assignment.StatusId == ActivityAssignmentStatusIds.Failed);

        var net = correct - incorrect;
        const Int32 start = MinAccomplishmentSize + range / 2;

        AccomplishmentSize = start + step * net;
    }

    private void SetGame(Game game)
    {
        GameActivityModels = game.GameActivities.Select(x => new GameActivityModel(x)).ToObservable();
        Game = game;
        _navigator.UpdateTitle(_path, Game.BookTitle!);
    }
}

public class SurveyModel : Observable
{
    private BookSurvey _survey;
    private SurveyStep _currentStep;

    public SurveyModel()
    {
        _survey = BookSurvey.HardcodedSurvey();
        _currentStep = _survey.Steps.First();
    }

    public BookSurvey Survey
    {
        get => _survey;
        set => SetProperty(ref _survey, value);
    }

    public SurveyStep CurrentStep
    {
        get => _currentStep;
        set => SetProperty(ref _currentStep, value);
    }
}

public class BookSurvey
{
    public Guid? Id { get; set; }
    public List<SurveyStep> Steps { get; set; } = [];

    public static BookSurvey HardcodedSurvey()
    {
        return new()
        {
            Id = Guid.Parse("d06bdfa8-1c05-4932-a61b-0a30454ba3e8"),
            Steps =
            [
                new()
                {
                    Id = AppSurveyIds.QuestionHowDifficultActivities,
                    Question = "How difficult were the activities?",
                    Prompt = "Please choose one.",
                    Choices =
                    [
                        new()
                        {
                            Id = AppSurveyIds.AnswerTooEasy,
                            Character = 128564,
                            Text = "Too easy",
                        },

                        new()
                        {
                            Id = AppSurveyIds.AnswerJustRight,
                            Character = 128578,
                            Text = "Just right",
                        },

                        new()
                        {
                            Id = AppSurveyIds.AnswerTooHard,
                            Character = 129402,
                            Text = "Too hard",
                        },
                    ],
                },

                new()
                {
                    Id = AppSurveyIds.QuestionHowDidYouLikeTheActivities,
                    Question = "How did you like the activities?",
                    Prompt = "Please choose one.",
                    Choices =
                    [
                        new()
                        {
                            Id = AppSurveyIds.AnswerLovedThem,
                            Character = 129392,
                            Text = "I loved them",
                        },

                        new()
                        {
                            Id = AppSurveyIds.AnswerLikedThem,
                            Character = 128512,
                            Text = "I liked them",
                        },

                        new()
                        {
                            Id = AppSurveyIds.AnswerTheyWereOkay,
                            Character = 128528,
                            Text = "They were okay",
                        },

                        new()
                        {
                            Id = AppSurveyIds.AnswerDidntLikeThem,
                            Character = 128577,
                            Text = "I didn't like them",
                        },
                    ],
                },

                new()
                {
                    Id = AppSurveyIds.QuestionHowDidThisMakeYouFeel,
                    Question = "How did this make you feel?",
                    Prompt = "Please choose one.",
                    Choices =
                    [
                        new()
                        {
                            Id = AppSurveyIds.AnswerGreatReader,
                            Character = 128526,
                            Text = "Like a great reader",
                        },

                        new()
                        {
                            Id = AppSurveyIds.AnswerGoodReader,
                            Character = 128578,
                            Text = "Like a good reader",
                        },

                        new()
                        {
                            Id = AppSurveyIds.AnswerOkayReader,
                            Character = 128533,
                            Text = "Like an okay reader",
                        },
                    ],
                },
            ],
        };
    }
}

public class SurveyStep
{
    public Guid? Id { get; set; }
    public String? Question { get; set; }
    public String? Prompt { get; set; }
    public List<SurveyChoice> Choices { get; set; } = [];
}

public class SurveyChoice
{
    public Guid? Id { get; set; }
    public Int32? Character { get; set; }
    public String? Text { get; set; }
}

public static class AppSurveyIds
{
    public static readonly Guid QuestionHowDifficultActivities = Guid.Parse("74d79956-ad72-42f5-ac9b-3c8bb0189150");
    public static readonly Guid AnswerTooEasy = Guid.Parse("f28f085e-87d5-4c5e-a80d-c0f59b01a076");
    public static readonly Guid AnswerJustRight = Guid.Parse("e315c1ae-1079-4ec4-b136-47f572b02025");
    public static readonly Guid AnswerTooHard = Guid.Parse("db6da0db-bd0b-4b3f-9a52-f3ca1c0d3400");

    public static readonly Guid QuestionHowDidYouLikeTheActivities = Guid.Parse("60dd4463-ee4e-4a47-a3ae-0f97327f23f7");
    public static readonly Guid AnswerLovedThem = Guid.Parse("83bc59a9-fad0-4471-b7bf-3d8994afcafd");
    public static readonly Guid AnswerLikedThem = Guid.Parse("d048e79d-79a4-4c39-ba34-a52b25659460");
    public static readonly Guid AnswerTheyWereOkay = Guid.Parse("7ba3df78-5637-4e1f-a709-26463f8d7f12");
    public static readonly Guid AnswerDidntLikeThem = Guid.Parse("39529c03-7909-4de3-9550-97f3be6ccd73");

    public static readonly Guid QuestionHowDidThisMakeYouFeel = Guid.Parse("c7f316d9-00fa-483d-8c6d-fd56bee9942e");
    public static readonly Guid AnswerGreatReader = Guid.Parse("99172f60-56ff-44e0-83bb-4f366db949e7");
    public static readonly Guid AnswerGoodReader = Guid.Parse("8a554297-40c7-493e-806a-9f362476d3e4");
    public static readonly Guid AnswerOkayReader = Guid.Parse("9d6be819-50c4-4e53-92cb-c875d32688da");
}