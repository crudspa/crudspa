using Crudspa.Education.Publisher.Client.Components;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class GameActivityEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IGameActivityService GameActivityService { get; set; } = null!;

    public GameActivityEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var sectionId = Path!.Id("game-section");

        Model = new(Path, Id, IsNew, sectionId, EventBus, Navigator, ScrollService, GameActivityService);
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

public class GameActivityEditModel : EditModel<GameActivity>, IHandle<GameActivitySaved>, IHandle<GameActivityRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _sectionId;
    private readonly INavigator _navigator;
    private readonly IGameActivityService _gameActivityService;
    public ActivityEditModel ActivityEditModel { get; }

    public GameActivityEditModel(String? path, Guid? id, Boolean isNew, Guid? sectionId,
        IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService,
        IGameActivityService gameActivityService) : base(isNew)
    {
        _path = path;
        _id = id;
        _sectionId = sectionId;
        _navigator = navigator;
        _gameActivityService = gameActivityService;

        ActivityEditModel = new(scrollService);

        eventBus.Subscribe(this);
    }

    public async Task Handle(GameActivitySaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(GameActivityRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<ActivityTypeFull> ActivityTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Named> ContentAreaNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ResearchGroupNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> GameActivityTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchActivityTypes(),
            FetchResearchGroupNames(),
            FetchContentAreaNames(),
            FetchGameActivityTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var id = Guid.NewGuid();

            SetGameActivity(new()
            {
                SectionId = _sectionId,
                Rigorous = false,
                Multisyllabic = false,
                ActivityId = id,
                Activity = new()
                {
                    Id = id,
                    ActivityTypeId = ActivityTypes.MinBy(x => x.Name)?.Id,
                    ContentAreaId = ContentAreaNames.MinBy(x => x.Name)?.Id,
                },
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _gameActivityService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetGameActivity(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _gameActivityService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/game-activity-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _gameActivityService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchActivityTypes()
    {
        var response = await WithAlerts(() => _gameActivityService.FetchActivityTypes(new()), false);
        if (response.Ok)
        {
            ActivityTypes = response.Value.ToList();
            ActivityEditModel.SetSelectedType();
        }
    }

    public async Task FetchContentAreaNames()
    {
        var response = await WithAlerts(() => _gameActivityService.FetchContentAreaNames(new()), false);
        if (response.Ok) ContentAreaNames = response.Value.ToList();
    }

    public async Task FetchResearchGroupNames()
    {
        var response = await WithAlerts(() => _gameActivityService.FetchResearchGroupNames(new()), false);
        if (response.Ok) ResearchGroupNames = response.Value.ToList();
    }

    public async Task FetchGameActivityTypeNames()
    {
        var response = await WithAlerts(() => _gameActivityService.FetchGameActivityTypeNames(new()), false);
        if (response.Ok) GameActivityTypeNames = response.Value.ToList();
    }

    private void SetGameActivity(GameActivity gameActivity)
    {
        Entity = gameActivity;

        if (ActivityEditModel.ActivityTypes.IsEmpty())
            ActivityEditModel.ActivityTypes = ActivityTypes;

        ActivityEditModel.SetActivity(gameActivity.Activity!);

        _navigator.UpdateTitle(_path, "Activity Editor");
    }
}