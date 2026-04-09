namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ObjectiveListForLesson : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IObjectiveService ObjectiveService { get; set; } = null!;

    public ObjectiveListForLessonModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ObjectiveService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ObjectiveListForLessonModel : ListOrderablesModel<ObjectiveModel>,
    IHandle<ObjectiveAdded>, IHandle<ObjectiveSaved>, IHandle<ObjectiveRemoved>, IHandle<ObjectivesReordered>,
    IHandle<PageAdded>, IHandle<PageRemoved>
{
    private readonly IObjectiveService _objectiveService;
    private readonly Guid? _lessonId;

    public ObjectiveListForLessonModel(IEventBus eventBus, IScrollService scrollService, IObjectiveService objectiveService, Guid? lessonId)
        : base(scrollService)
    {
        _objectiveService = objectiveService;

        _lessonId = lessonId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ObjectiveAdded payload) => await Replace(payload.Id, payload.LessonId);

    public async Task Handle(ObjectiveSaved payload) => await Replace(payload.Id, payload.LessonId);

    public async Task Handle(ObjectiveRemoved payload) => await Rid(payload.Id, payload.LessonId);

    public async Task Handle(ObjectivesReordered payload) => await Refresh();

    public async Task Handle(PageAdded payload) => await Refresh(false);

    public async Task Handle(PageRemoved payload) => await Refresh(false);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Lesson>(new() { Id = _lessonId });
        var response = await WithWaiting("Fetching...", () => _objectiveService.FetchForLesson(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ObjectiveModel(x)).ToList());
    }

    public override async Task<Response<ObjectiveModel?>> Fetch(Guid? id)
    {
        var response = await _objectiveService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ObjectiveModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _objectiveService.Remove(new(new()
        {
            Id = id,
            LessonId = _lessonId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_lessonId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Objective).ToList();
        return await WithWaiting("Saving...", () => _objectiveService.SaveOrder(new(orderables)));
    }
}

public class ObjectiveModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleObjectiveChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Objective));

    private Objective _objective;

    public String? Name => Objective.Title;

    public ObjectiveModel(Objective objective)
    {
        _objective = objective;
        _objective.PropertyChanged += HandleObjectiveChanged;
    }

    public void Dispose()
    {
        _objective.PropertyChanged -= HandleObjectiveChanged;
    }

    public Guid? Id
    {
        get => _objective.Id;
        set => _objective.Id = value;
    }

    public Int32? Ordinal
    {
        get => _objective.Ordinal;
        set => _objective.Ordinal = value;
    }

    public Objective Objective
    {
        get => _objective;
        set => SetProperty(ref _objective, value);
    }
}