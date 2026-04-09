namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class CourseEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ICourseService CourseService { get; set; } = null!;

    public CourseEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var trackId = Path!.Id("track");
        var portalId = Path!.Id("portal");

        Model = new(Path, Id, IsNew, trackId, portalId, EventBus, Navigator, CourseService);
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

public class CourseEditModel : EditModel<Course>, IHandle<CourseSaved>, IHandle<CourseRemoved>, IHandle<CoursesReordered>
    , IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _trackId;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly ICourseService _courseService;

    public CourseEditModel(String? path, Guid? id, Boolean isNew, Guid? trackId, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        ICourseService courseService) : base(isNew)
    {
        _path = path;
        _id = id;
        _trackId = trackId;
        _portalId = portalId;
        _navigator = navigator;
        _courseService = courseService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CourseSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(CourseRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(CoursesReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _courseService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title!);
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

            SetCourse(new()
            {
                TrackId = _trackId,
                Title = "New Course",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                Description = String.Empty,
                Binder = new()
                {
                    TypeId = BinderTypeNames.MinBy(x => x.Ordinal)?.Id,
                },
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _courseService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetCourse(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _courseService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/course-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _courseService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _courseService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchAchievementNames()
    {
        var response = await WithAlerts(() => _courseService.FetchAchievementNames(new(new() { Id = _portalId })), false);
        if (response.Ok) AchievementNames = response.Value.ToObservable();
    }

    public async Task FetchBinderTypeNames()
    {
        var response = await WithAlerts(() => _courseService.FetchBinderTypeNames(new()), false);
        if (response.Ok) BinderTypeNames = response.Value.ToList();
    }

    private void SetCourse(Course course)
    {
        Entity = course;
        _navigator.UpdateTitle(_path, Entity.Title!);
    }
}