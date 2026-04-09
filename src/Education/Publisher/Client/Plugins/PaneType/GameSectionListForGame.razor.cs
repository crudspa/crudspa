namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class GameSectionListForGame : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IGameSectionService GameSectionService { get; set; } = null!;

    public GameSectionListForGameModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, GameSectionService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class GameSectionListForGameModel : ListOrderablesModel<GameSectionModel>,
    IHandle<GameSectionAdded>, IHandle<GameSectionSaved>, IHandle<GameSectionRemoved>, IHandle<GameSectionsReordered>,
    IHandle<GameActivityAdded>, IHandle<GameActivityRemoved>
{
    private readonly IGameSectionService _gameSectionService;
    private readonly Guid? _gameId;

    public GameSectionListForGameModel(IEventBus eventBus, IScrollService scrollService, IGameSectionService gameSectionService, Guid? gameId)
        : base(scrollService)
    {
        _gameSectionService = gameSectionService;
        _gameId = gameId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(GameSectionAdded payload) => await Replace(payload.Id, payload.GameId);
    public async Task Handle(GameSectionSaved payload) => await Replace(payload.Id, payload.GameId);
    public async Task Handle(GameSectionRemoved payload) => await Rid(payload.Id, payload.GameId);

    public async Task Handle(GameSectionsReordered payload) => await Refresh();

    public async Task Handle(GameActivityAdded payload)
    {
        if (payload.SectionId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.SectionId)))
            await Replace(payload.SectionId);
    }

    public async Task Handle(GameActivityRemoved payload)
    {
        if (payload.SectionId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.SectionId)))
            await Replace(payload.SectionId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Game>(new() { Id = _gameId });
        var response = await WithWaiting("Fetching...", () => _gameSectionService.FetchForGame(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new GameSectionModel(x)).ToList());
    }

    public override async Task<Response<GameSectionModel?>> Fetch(Guid? id)
    {
        var response = await _gameSectionService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new GameSectionModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _gameSectionService.Remove(new(new()
        {
            Id = id,
            GameId = _gameId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_gameId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.GameSection).ToList();
        return await WithWaiting("Saving...", () => _gameSectionService.SaveOrder(new(orderables)));
    }
}

public class GameSectionModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleGameSectionChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(GameSection));

    private GameSection _gameSection;

    public String? Name => GameSection.Name;

    public GameSectionModel(GameSection gameSection)
    {
        _gameSection = gameSection;
        _gameSection.PropertyChanged += HandleGameSectionChanged;
    }

    public void Dispose()
    {
        _gameSection.PropertyChanged -= HandleGameSectionChanged;
    }

    public Guid? Id
    {
        get => _gameSection.Id;
        set => _gameSection.Id = value;
    }

    public Int32? Ordinal
    {
        get => _gameSection.Ordinal;
        set => _gameSection.Ordinal = value;
    }

    public GameSection GameSection
    {
        get => _gameSection;
        set => SetProperty(ref _gameSection, value);
    }
}