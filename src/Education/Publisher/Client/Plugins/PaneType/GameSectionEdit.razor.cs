namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class GameSectionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IGameSectionService GameSectionService { get; set; } = null!;

    public GameSectionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var gameId = Path!.Id("game");

        Model = new(Path, Id, IsNew, gameId, EventBus, Navigator, GameSectionService);
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

public class GameSectionEditModel : EditModel<GameSection>,
    IHandle<GameSectionSaved>, IHandle<GameSectionRemoved>, IHandle<GameSectionsReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _gameId;
    private readonly INavigator _navigator;
    private readonly IGameSectionService _gameSectionService;

    public GameSectionEditModel(String? path, Guid? id, Boolean isNew, Guid? gameId,
        IEventBus eventBus,
        INavigator navigator,
        IGameSectionService gameSectionService) : base(isNew)
    {
        _path = path;
        _id = id;
        _gameId = gameId;
        _navigator = navigator;
        _gameSectionService = gameSectionService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(GameSectionSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(GameSectionRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(GameSectionsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _gameSectionService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Name);
        }
    }

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> GameSectionTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchGameSectionTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetGameSection(new()
            {
                GameId = _gameId,
                Title = "New Section",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                TypeId = GameSectionTypeNames.MinBy(x => x.Ordinal)?.Id,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _gameSectionService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetGameSection(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _gameSectionService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/game-section-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _gameSectionService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _gameSectionService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchGameSectionTypeNames()
    {
        var response = await WithAlerts(() => _gameSectionService.FetchGameSectionTypeNames(new()), false);
        if (response.Ok) GameSectionTypeNames = response.Value.ToList();
    }

    private void SetGameSection(GameSection gameSection)
    {
        Entity = gameSection;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}