namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class ObjectiveDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public IObjectiveProgressService ObjectiveProgressService { get; set; } = null!;

    public ObjectiveDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, StudentAppService, ObjectiveProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ObjectiveDisplayModel : ScreenModel
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IStudentAppService _studentAppService;
    private readonly IObjectiveProgressService _objectiveProgressService;
    private Objective? _objective;

    public ObjectiveDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IStudentAppService studentAppService,
        IObjectiveProgressService objectiveProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _studentAppService = studentAppService;
        _objectiveProgressService = objectiveProgressService;

        eventBus.Subscribe(this);
    }

    public Objective? Objective
    {
        get => _objective;
        set => SetProperty(ref _objective, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchObjective(new(new() { Id = _id })));

        if (response.Ok)
            await SetObjective(response.Value);
    }

    public async Task HandleBinderCompleted()
    {
        await _objectiveProgressService.AddCompleted(new(new()
        {
            ObjectiveId = _objective!.Id,
            DeviceTimestamp = DateTimeOffset.Now,
        }));

        _navigator.Close(_path);
    }

    private Task SetObjective(Objective objective)
    {
        Objective = objective;

        _navigator.UpdateTitle(_path, Objective.Title!);

        return Task.CompletedTask;
    }
}