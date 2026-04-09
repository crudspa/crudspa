namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class LessonListForUnit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ILessonService LessonService { get; set; } = null!;

    public LessonListForUnitModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, LessonService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class LessonListForUnitModel : ListOrderablesModel<LessonModel>,
    IHandle<LessonAdded>, IHandle<LessonSaved>, IHandle<LessonRemoved>, IHandle<LessonsReordered>,
    IHandle<ObjectiveAdded>, IHandle<ObjectiveRemoved>
{
    private readonly ILessonService _lessonService;
    private readonly Guid? _unitId;

    public LessonListForUnitModel(IEventBus eventBus, IScrollService scrollService, ILessonService lessonService, Guid? unitId)
        : base(scrollService)
    {
        _lessonService = lessonService;
        _unitId = unitId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(LessonAdded payload) => await Replace(payload.Id, payload.UnitId);

    public async Task Handle(LessonSaved payload) => await Replace(payload.Id, payload.UnitId);

    public async Task Handle(LessonRemoved payload) => await Rid(payload.Id, payload.UnitId);

    public async Task Handle(LessonsReordered payload) => await Refresh();

    public async Task Handle(ObjectiveAdded payload)
    {
        if (payload.LessonId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.LessonId)))
            await Replace(payload.LessonId);
    }

    public async Task Handle(ObjectiveRemoved payload)
    {
        if (payload.LessonId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.LessonId)))
            await Replace(payload.LessonId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Unit>(new() { Id = _unitId });
        var response = await WithWaiting("Fetching...", () => _lessonService.FetchForUnit(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new LessonModel(x)).ToList());
    }

    public override async Task<Response<LessonModel?>> Fetch(Guid? id)
    {
        var response = await _lessonService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new LessonModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _lessonService.Remove(new(new()
        {
            Id = id,
            UnitId = _unitId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_unitId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Lesson).ToList();
        return await WithWaiting("Saving...", () => _lessonService.SaveOrder(new(orderables)));
    }
}

public class LessonModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleLessonChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Lesson));

    private Lesson _lesson;

    public String? Name => Lesson.Title;

    public LessonModel(Lesson lesson)
    {
        _lesson = lesson;
        _lesson.PropertyChanged += HandleLessonChanged;
    }

    public void Dispose()
    {
        _lesson.PropertyChanged -= HandleLessonChanged;
    }

    public Guid? Id
    {
        get => _lesson.Id;
        set => _lesson.Id = value;
    }

    public Int32? Ordinal
    {
        get => _lesson.Ordinal;
        set => _lesson.Ordinal = value;
    }

    public Lesson Lesson
    {
        get => _lesson;
        set => SetProperty(ref _lesson, value);
    }
}