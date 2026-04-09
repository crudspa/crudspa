namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class CourseListForTrack : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ICourseService CourseService { get; set; } = null!;

    public CourseListForTrackModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, CourseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CourseListForTrackModel : ListOrderablesModel<CourseModel>,
    IHandle<CourseAdded>, IHandle<CourseSaved>, IHandle<CourseRemoved>,
    IHandle<PageAdded>, IHandle<PageRemoved>
{
    private readonly ICourseService _courseService;
    private readonly Guid? _trackId;

    public CourseListForTrackModel(IEventBus eventBus, IScrollService scrollService, ICourseService courseService, Guid? trackId)
        : base(scrollService)
    {
        _courseService = courseService;
        _trackId = trackId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CourseAdded payload) => await Replace(payload.Id, payload.TrackId);
    public async Task Handle(CourseSaved payload) => await Replace(payload.Id, payload.TrackId);
    public async Task Handle(CourseRemoved payload) => await Rid(payload.Id, payload.TrackId);
    public async Task Handle(PageAdded payload) => await Refresh(false);
    public async Task Handle(PageRemoved payload) => await Refresh(false);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Track>(new() { Id = _trackId });
        var response = await WithWaiting("Fetching...", () => _courseService.FetchForTrack(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new CourseModel(x)).ToList());
    }

    public override async Task<Response<CourseModel?>> Fetch(Guid? id)
    {
        var response = await _courseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new CourseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _courseService.Remove(new(new()
        {
            Id = id,
            TrackId = _trackId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_trackId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Course).ToList();
        return await WithWaiting("Saving...", () => _courseService.SaveOrder(new(orderables)));
    }
}

public class CourseModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleCourseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Course));

    private Course _course;

    public String? Name => Course.Title;

    public CourseModel(Course course)
    {
        _course = course;
        _course.PropertyChanged += HandleCourseChanged;
    }

    public void Dispose()
    {
        _course.PropertyChanged -= HandleCourseChanged;
    }

    public Guid? Id
    {
        get => _course.Id;
        set => _course.Id = value;
    }

    public Int32? Ordinal
    {
        get => _course.Ordinal;
        set => _course.Ordinal = value;
    }

    public Course Course
    {
        get => _course;
        set => SetProperty(ref _course, value);
    }
}