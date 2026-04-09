namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class TrackDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ICourseRunService CourseRunService { get; set; } = null!;

    public TrackDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<TrackConfig>();

        if (config is not null && config.IdSource == TrackConfig.IdSources.SpecificTrack && config.TrackId.HasSomething())
            Id = config.TrackId;

        Model = new(Path, Id, EventBus, Navigator, CourseRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class TrackDisplayModel : ScreenModel,
    IHandle<CourseProgressUpdated>,
    IHandle<ContactAchievementAdded>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ICourseRunService _courseRunService;

    public TrackDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        ICourseRunService courseRunService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _courseRunService = courseRunService;

        eventBus.Subscribe(this);
    }

    public Track? Track
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _courseRunService.FetchTrack(new(new() { Id = _id })));

        if (response.Ok)
            await SetTrack(response.Value);
    }

    public Task Handle(CourseProgressUpdated payload)
    {
        if (Track is null)
            return Task.CompletedTask;

        foreach (var course in Track.Courses)
            if (course.Id.Equals(payload.Progress.CourseId))
            {
                course.Progress = payload.Progress;
                break;
            }

        EvaluateProgress();

        return Task.CompletedTask;
    }

    public async Task Handle(ContactAchievementAdded payload)
    {
        await Refresh();
    }

    public void GoToCourse(Guid? id)
    {
        _navigator.GoTo($"{_path}/course-{id:D}");
    }

    private void EvaluateProgress()
    {
        if (Track is null)
            return;

        if (Track.RequireSequentialCompletion == true)
        {
            var previousCompleted = true;
            foreach (var course in Track.Courses)
            {
                course.Locked = !previousCompleted;
                previousCompleted = course.Progress.TimesCompleted > 0;
            }
        }

        RaisePropertyChanged(nameof(Track));
    }

    private async Task SetTrack(Track track)
    {
        foreach (var course in track.Courses)
            course.Progress = await _courseRunService.FetchProgress(new(new() { Id = course.Id }));

        Track = track;

        _navigator.UpdateTitle(_path, Track.Title!);

        EvaluateProgress();
    }
}