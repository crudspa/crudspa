using AchievementAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementAdded;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ObjectiveEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IObjectiveService ObjectiveService { get; set; } = null!;

    public ObjectiveEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var lessonId = Path!.Id("lesson");

        Model = new(Path, Id, IsNew, lessonId, EventBus, Navigator, ObjectiveService);
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

public class ObjectiveEditModel : EditModel<Objective>,
    IHandle<ObjectiveSaved>, IHandle<ObjectiveRemoved>, IHandle<ObjectivesReordered>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _lessonId;
    private readonly INavigator _navigator;
    private readonly IObjectiveService _objectiveService;

    public ObjectiveEditModel(String? path, Guid? id, Boolean isNew, Guid? lessonId,
        IEventBus eventBus,
        INavigator navigator,
        IObjectiveService objectiveService) : base(isNew)
    {
        _path = path;
        _id = id;
        _lessonId = lessonId;
        _navigator = navigator;
        _objectiveService = objectiveService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ObjectiveSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ObjectiveRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(ObjectivesReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _objectiveService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title);
        }
    }

    public async Task Handle(AchievementAdded payload) => await FetchAchievementNames();

    public async Task Handle(AchievementSaved payload) => await FetchAchievementNames();

    public async Task Handle(AchievementRemoved payload) => await FetchAchievementNames();

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> AchievementNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> BinderTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchAchievementNames(),
            FetchBinderTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetObjective(new()
            {
                LessonId = _lessonId,
                Title = "New Objective",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                Binder = new()
                {
                    TypeId = BinderTypeNames.MinBy(x => x.Ordinal)?.Id,
                },
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _objectiveService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetObjective(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _objectiveService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/objective-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _objectiveService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _objectiveService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchAchievementNames()
    {
        var response = await WithAlerts(() => _objectiveService.FetchAchievementNames(new()), false);
        if (response.Ok) AchievementNames = response.Value.ToObservable();
    }

    public async Task FetchBinderTypeNames()
    {
        var response = await WithAlerts(() => _objectiveService.FetchBinderTypeNames(new()), false);
        if (response.Ok) BinderTypeNames = response.Value.ToList();
    }

    private void SetObjective(Objective objective)
    {
        Entity = objective;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}