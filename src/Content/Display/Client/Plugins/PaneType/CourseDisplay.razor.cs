namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class CourseDisplay : IPaneDisplay, IHasPaneId, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? PaneId { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ICourseRunService CourseRunService { get; set; } = null!;

    public CourseDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, PaneId, EventBus, Navigator, CourseRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CourseDisplayModel : ScreenModel
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _paneId;
    private readonly INavigator _navigator;
    private readonly ICourseRunService _courseRunService;
    private Course? _course;

    public CourseDisplayModel(String? path, Guid? id, Guid? paneId, IEventBus eventBus,
        INavigator navigator,
        ICourseRunService courseRunService)
    {
        _path = path;
        _id = id;
        _paneId = paneId;
        _navigator = navigator;
        _courseRunService = courseRunService;

        eventBus.Subscribe(this);
    }

    public Course? Course
    {
        get => _course;
        set => SetProperty(ref _course, value);
    }

    public async Task Refresh()
    {
        var response = _paneId.HasValue
            ? await WithWaiting("Loading...", () => _courseRunService.FetchCourseForPane(new(new() { PaneId = _paneId, RouteCourseId = _id })))
            : await WithWaiting("Loading...", () => _courseRunService.FetchCourse(new(new() { Id = _id })));

        if (response.Ok && response.Value is not null)
            await SetCourse(response.Value);
    }

    public async Task HandleBinderCompleted()
    {
        await _courseRunService.AddCompleted(new(new()
        {
            CourseId = _course!.Id,
        }));
    }

    private Task SetCourse(Course course)
    {
        Course = course;

        _navigator.UpdateTitle(_path, Course.Title!);

        return Task.CompletedTask;
    }
}