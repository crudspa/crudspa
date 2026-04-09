using AchievementAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementAdded;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class GameEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IGameService GameService { get; set; } = null!;

    public GameEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var bookId = Path!.Id("book");

        Model = new(Path, Id, IsNew, bookId, EventBus, Navigator, GameService);
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

public class GameEditModel : EditModel<Game>,
    IHandle<GameSaved>, IHandle<GameRemoved>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _bookId;
    private readonly INavigator _navigator;
    private readonly IGameService _gameService;

    public GameEditModel(String? path, Guid? id, Boolean isNew, Guid? bookId,
        IEventBus eventBus,
        INavigator navigator,
        IGameService gameService) : base(isNew)
    {
        _path = path;
        _id = id;
        _bookId = bookId;
        _navigator = navigator;
        _gameService = gameService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(GameSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(GameRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(AchievementAdded payload) => await FetchAchievementNames();

    public async Task Handle(AchievementSaved payload) => await FetchAchievementNames();

    public async Task Handle(AchievementRemoved payload) => await FetchAchievementNames();

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<IconFull> Icons
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> GradeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> AssessmentLevelNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> AchievementNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchIcons(),
            FetchGradeNames(),
            FetchAssessmentLevelNames(),
            FetchAchievementNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetGame(new()
            {
                BookId = _bookId,
                Key = String.Empty,
                Title = "New Game",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                GradeId = GradeNames.MinBy(x => x.Ordinal)?.Id,
                AssessmentLevelId = AssessmentLevelNames.MinBy(x => x.Ordinal)?.Id,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _gameService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetGame(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _gameService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/game-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _gameService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _gameService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchIcons()
    {
        var response = await WithAlerts(() => _gameService.FetchIcons(new()), false);
        if (response.Ok) Icons = response.Value.ToList();
    }

    public async Task FetchGradeNames()
    {
        var response = await WithAlerts(() => _gameService.FetchGradeNames(new()), false);
        if (response.Ok) GradeNames = response.Value.ToList();
    }

    public async Task FetchAssessmentLevelNames()
    {
        var response = await WithAlerts(() => _gameService.FetchAssessmentLevelNames(new()), false);
        if (response.Ok) AssessmentLevelNames = response.Value.ToList();
    }

    public async Task FetchAchievementNames()
    {
        var response = await WithAlerts(() => _gameService.FetchAchievementNames(new()), false);
        if (response.Ok) AchievementNames = response.Value.ToObservable();
    }

    private void SetGame(Game game)
    {
        Entity = game;
        _navigator.UpdateTitle(_path, Entity.Key);
    }
}