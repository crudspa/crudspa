namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class TrackList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ICourseRunService CourseRunService { get; set; } = null!;

    public TrackListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, EventBus, Navigator, CourseRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class TrackListModel : ScreenModel,
    IHandle<CourseProgressUpdated>
{
    private readonly String? _path;
    private readonly INavigator _navigator;
    private readonly ICourseRunService _courseRunService;

    public TrackListModel(String? path, IEventBus eventBus,
        INavigator navigator,
        ICourseRunService courseRunService)
    {
        _path = path;
        _navigator = navigator;
        _courseRunService = courseRunService;

        eventBus.Subscribe(this);
    }

    public PortalTracks? Portal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _courseRunService.FetchPortalTracks(new()));

        if (response.Ok)
            await SetPortal(response.Value);
    }

    public void GoToTrack(Guid? id)
    {
        _navigator.GoTo($"{_path}/track-{id:D}");
    }

    public Task Handle(CourseProgressUpdated payload)
    {
        if (Portal is null)
            return Task.CompletedTask;

        foreach (var track in Portal.Tracks)
        foreach (var course in track.Courses)
            if (course.Id.Equals(payload.Progress.CourseId))
            {
                course.Progress = payload.Progress;
                break;
            }

        RaisePropertyChanged(nameof(Portal));

        return Task.CompletedTask;
    }

    public async Task Handle(ContactAchievementAdded payload)
    {
        await Refresh();
    }

    private async Task SetPortal(PortalTracks portal)
    {
        foreach (var track in portal.Tracks)
        foreach (var course in track.Courses)
            course.Progress = await _courseRunService.FetchProgress(new(new() { Id = course.Id }));

        Portal = portal;
    }
}