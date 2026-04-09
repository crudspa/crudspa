namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class UnitDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public IBookProgressService BookProgressService { get; set; } = null!;
    [Inject] public IGameProgressService GameProgressService { get; set; } = null!;
    [Inject] public IModuleProgressService ModuleProgressService { get; set; } = null!;
    [Inject] public IObjectiveProgressService ObjectiveProgressService { get; set; } = null!;

    public UnitDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, SoundService, StudentAppService, BookProgressService, GameProgressService, ModuleProgressService, ObjectiveProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class UnitDisplayModel : ScreenModel,
    IHandle<BookProgressUpdated>,
    IHandle<GameProgressUpdated>,
    IHandle<ModuleProgressUpdated>,
    IHandle<ObjectiveProgressUpdated>,
    IHandle<StudentAchievementAdded>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ISoundService _soundService;
    private readonly IStudentAppService _studentAppService;
    private readonly IBookProgressService _bookProgressService;
    private readonly IGameProgressService _gameProgressService;
    private readonly IModuleProgressService _moduleProgressService;
    private readonly IObjectiveProgressService _objectiveProgressService;

    public UnitDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        ISoundService soundService,
        IStudentAppService studentAppService,
        IBookProgressService bookProgressService,
        IGameProgressService gameProgressService,
        IModuleProgressService moduleProgressService,
        IObjectiveProgressService objectiveProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _soundService = soundService;
        _studentAppService = studentAppService;
        _bookProgressService = bookProgressService;
        _gameProgressService = gameProgressService;
        _moduleProgressService = moduleProgressService;
        _objectiveProgressService = objectiveProgressService;

        eventBus.Subscribe(this);
    }

    public Unit? Unit
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var objectiveProgressTask = _objectiveProgressService.FetchAll(new());
        var bookProgressTask = _bookProgressService.FetchAll(new());
        var gameProgressTask = _gameProgressService.FetchAll(new());
        var moduleProgressTask = _moduleProgressService.FetchAll(new());

        var response = await WithWaiting("Loading...", async () =>
        {
            var unitTask = _studentAppService.FetchUnit(new(new() { Id = _id }));

            await Task.WhenAll(unitTask, objectiveProgressTask, bookProgressTask, gameProgressTask, moduleProgressTask);

            return await unitTask;
        });

        if (response.Ok)
            SetUnit(response.Value, await objectiveProgressTask, await bookProgressTask, await gameProgressTask, await moduleProgressTask);
    }

    public void GoToUnit(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/unit-{id:D}");
    }

    public void GoToLesson(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/lesson-{id:D}");
    }

    public void GoToBook(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/book-{id:D}");
    }

    public Task Handle(BookProgressUpdated payload)
    {
        if (Unit is null)
            return Task.CompletedTask;

        foreach (var unitBook in Unit.UnitBookSummaries)
        {
            if (unitBook.BookId.Equals(payload.Progress.BookId))
            {
                unitBook.Book!.Progress = payload.Progress;
                RaisePropertyChanged(nameof(Unit));
                break;
            }
        }

        return Task.CompletedTask;
    }

    public Task Handle(GameProgressUpdated payload)
    {
        if (Unit is null)
            return Task.CompletedTask;

        foreach (var unitBook in Unit.UnitBookSummaries)
        {
            foreach (var game in unitBook.Book!.Games)
            {
                if (game.Id.Equals(payload.Progress.GameId))
                {
                    game.Progress = payload.Progress;
                    RaisePropertyChanged(nameof(Unit));
                    break;
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task Handle(ModuleProgressUpdated payload)
    {
        if (Unit is null)
            return Task.CompletedTask;

        foreach (var unitBook in Unit.UnitBookSummaries)
        {
            foreach (var module in unitBook.Book!.Modules)
            {
                if (module.Id.Equals(payload.Progress.ModuleId))
                {
                    module.Progress = payload.Progress;
                    RaisePropertyChanged(nameof(Unit));
                    break;
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task Handle(ObjectiveProgressUpdated payload)
    {
        if (Unit is null)
            return Task.CompletedTask;

        foreach (var lesson in Unit.LessonSummaries)
        foreach (var objective in lesson.Objectives)
        {
            if (objective.Id.Equals(payload.Progress.ObjectiveId))
            {
                objective.Progress = payload.Progress;
                RaisePropertyChanged(nameof(Unit));
                break;
            }
        }

        return Task.CompletedTask;
    }

    public async Task Handle(StudentAchievementAdded payload)
    {
        await Refresh();
    }

    private void SetUnit(
        Unit unit,
        Response<IList<ObjectiveProgress>> objectiveProgressResponse,
        Response<IList<BookProgress>> bookProgressResponse,
        Response<IList<GameProgress>> gameProgressResponse,
        Response<IList<ModuleProgress>> moduleProgressResponse)
    {
        var objectiveProgressByObjectiveId = objectiveProgressResponse.Ok
            ? objectiveProgressResponse.Value
                .Where(x => x.ObjectiveId.HasValue)
                .ToDictionary(x => x.ObjectiveId!.Value)
            : new Dictionary<Guid, ObjectiveProgress>();

        var bookProgressByBookId = bookProgressResponse.Ok
            ? bookProgressResponse.Value
                .Where(x => x.BookId.HasValue)
                .ToDictionary(x => x.BookId!.Value)
            : new Dictionary<Guid, BookProgress>();

        var gameProgressByGameId = gameProgressResponse.Ok
            ? gameProgressResponse.Value
                .Where(x => x.GameId.HasValue)
                .ToDictionary(x => x.GameId!.Value)
            : new Dictionary<Guid, GameProgress>();

        var moduleProgressByModuleId = moduleProgressResponse.Ok
            ? moduleProgressResponse.Value
                .Where(x => x.ModuleId.HasValue)
                .ToDictionary(x => x.ModuleId!.Value)
            : new Dictionary<Guid, ModuleProgress>();

        foreach (var lesson in unit.LessonSummaries)
        foreach (var objective in lesson.Objectives)
            objective.Progress = objective.Id is Guid objectiveId
                && objectiveProgressByObjectiveId.TryGetValue(objectiveId, out var objectiveProgress)
                    ? objectiveProgress
                    : new()
                    {
                        ObjectiveId = objective.Id,
                        TimesCompleted = 0,
                    };

        foreach (var unitBook in unit.UnitBookSummaries)
        {
            unitBook.Book!.Progress = unitBook.BookId is Guid bookId
                && bookProgressByBookId.TryGetValue(bookId, out var bookProgress)
                    ? bookProgress
                    : new()
                    {
                        BookId = unitBook.BookId,
                        PrefaceCompletedCount = 0,
                        ContentCompletedCount = 0,
                        MapCompletedCount = 0,
                    };

            foreach (var game in unitBook.Book.Games)
                game.Progress = game.Id is Guid gameId
                    && gameProgressByGameId.TryGetValue(gameId, out var gameProgress)
                        ? gameProgress
                        : new()
                        {
                            GameId = game.Id,
                            GameCompletedCount = 0,
                        };

            foreach (var module in unitBook.Book.Modules)
                module.Progress = module.Id is Guid moduleId
                    && moduleProgressByModuleId.TryGetValue(moduleId, out var moduleProgress)
                        ? moduleProgress
                        : new()
                        {
                            ModuleId = module.Id,
                            ModuleCompletedCount = 0,
                        };
        }

        Unit = unit;
        _navigator.UpdateTitle(_path, Unit.Title!);
    }
}