namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class GameFindForBook : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IGameService GameService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public GameFindForBookModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, GameService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void AddNew()
    {
        Navigator.GoTo($"{Path}/game-{Guid.NewGuid():D}?state=new");
    }
}

public class GameFindForBookModel : FindModel<GameSearch, Game>,
    IHandle<GameAdded>, IHandle<GameSaved>, IHandle<GameRemoved>,
    IHandle<GameSectionAdded>, IHandle<GameSectionRemoved>
{
    private readonly IGameService _gameService;
    private readonly Guid? _bookId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public GameFindForBookModel(IEventBus eventBus, IScrollService scrollService, IGameService gameService, Guid? bookId)
        : base(scrollService)
    {
        _gameService = gameService;
        _bookId = bookId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Key",
            "Title",
        ];
    }

    public async Task Handle(GameAdded payload) => await Refresh();

    public async Task Handle(GameSaved payload) => await Refresh();

    public async Task Handle(GameRemoved payload) => await Refresh();

    public async Task Handle(GameSectionAdded payload) => await Refresh();

    public async Task Handle(GameSectionRemoved payload) => await Refresh();

    public List<Orderable> ContentStatusNames
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

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _bookId;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = true;
        Search.Status.Clear();
        Search.Grades.Clear();
        Search.AssessmentLevels.Clear();

        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchGradeNames(),
            FetchAssessmentLevelNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<GameSearch>(Search);
        var response = await WithWaiting("Searching...", () => _gameService.SearchForBook(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _gameService.Remove(new(new() { Id = id })));
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _gameService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
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
}