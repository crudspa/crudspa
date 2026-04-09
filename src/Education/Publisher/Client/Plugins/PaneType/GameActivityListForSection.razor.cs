namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class GameActivityListForSection : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IGameActivityService GameActivityService { get; set; } = null!;

    public GameActivityListForSectionModel Model { get; set; } = null!;
    public Guid PreviewPortalId => PortalIds.Student;
    public Boolean ThemeReady { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, GameActivityService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private Task HandleThemeLoaded()
    {
        ThemeReady = true;
        return InvokeAsync(StateHasChanged);
    }
}

public class GameActivityListForSectionModel : ListOrderablesModel<GameActivityModel>,
    IHandle<GameActivityAdded>, IHandle<GameActivitySaved>, IHandle<GameActivityRemoved>, IHandle<GameActivityShared>
{
    private readonly IGameActivityService _gameActivityService;
    private readonly Guid? _sectionId;

    public ModalModel ShareModel;

    public GameActivityListForSectionModel(IEventBus eventBus, IScrollService scrollService, IGameActivityService gameActivityService, Guid? sectionId)
        : base(scrollService)
    {
        _gameActivityService = gameActivityService;
        _sectionId = sectionId;

        ShareModel = new(scrollService);

        eventBus.Subscribe(this);
    }

    public async Task Handle(GameActivityAdded payload) => await Replace(payload.Id, payload.SectionId);
    public async Task Handle(GameActivitySaved payload) => await Replace(payload.Id, payload.SectionId);
    public async Task Handle(GameActivityRemoved payload) => await Rid(payload.Id, payload.SectionId);
    public async Task Handle(GameActivityShared payload) => await Refresh(false);

    public ObservableCollection<Named> Books
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> Sections
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Guid? SelectedGameActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SelectedSectionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SelectedBookId
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
                _ = FetchSections();
        }
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchBookNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<GameSection>(new() { Id = _sectionId });
        var response = await WithWaiting("Fetching...", () => _gameActivityService.FetchForSection(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new GameActivityModel(x)).ToList());
    }

    public override async Task<Response<GameActivityModel?>> Fetch(Guid? id)
    {
        var response = await _gameActivityService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new GameActivityModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _gameActivityService.Remove(new(new()
        {
            Id = id,
            SectionId = _sectionId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_sectionId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.GameActivity).ToList();
        return await WithWaiting("Saving...", () => _gameActivityService.SaveOrder(new(orderables)));
    }

    public async Task FetchBookNames()
    {
        var response = await WithAlerts(() => _gameActivityService.FetchBooks(new()), false);
        if (response.Ok) Books = response.Value.ToObservable();
    }

    public async Task Share()
    {
        await ShareModel.Hide();

        await WithAlerts(() => _gameActivityService.Share(new(new()
        {
            SourceGameActivityId = SelectedGameActivityId,
            TargetGameSectionId = SelectedSectionId,
        })));
    }

    public async Task StartShare(Guid? id)
    {
        SelectedGameActivityId = id;
        SelectedBookId = Books.OrderBy(x => x.Name).First().Id;

        await ShareModel.Show();

        await FetchSections();
    }

    private async Task FetchSections()
    {
        var response = await WithAlerts(() => _gameActivityService.FetchSections(new(new() { Id = SelectedBookId })), false);

        if (response.Ok)
        {
            Sections = response.Value.ToObservable();
            SelectedSectionId = Sections.First().Id;
        }
    }
}