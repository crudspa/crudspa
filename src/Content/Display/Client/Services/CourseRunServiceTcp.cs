namespace Crudspa.Content.Display.Client.Services;

public class CourseRunServiceTcp : ICourseRunService, IHandle<CourseProgressUpdated>
{
    private readonly IProxyWrappers _proxyWrappers;
    private readonly Lazy<Task> _lazyCourses;
    private readonly Object _coursesLock = new();
    private readonly SemaphoreSlim _initLock = new(1);
    private List<CourseProgress> _courses = [];

    public CourseRunServiceTcp(IProxyWrappers proxyWrappers, IEventBus eventBus)
    {
        _proxyWrappers = proxyWrappers;
        _lazyCourses = new(FetchCoursesFromServer);

        eventBus.Subscribe(this);
    }

    public async Task<Response<Course?>> FetchCourse(Request<Course> request) =>
        await _proxyWrappers.Send<Course?>("CourseRunFetchCourse", request);

    public async Task<Response<Track?>> FetchTrack(Request<Track> request) =>
        await _proxyWrappers.Send<Track?>("CourseRunFetchTrack", request);

    public async Task<Response<PortalTracks?>> FetchPortalTracks(Request request) =>
        await _proxyWrappers.Send<PortalTracks?>("CourseRunFetchPortalTracks", request);

    public async Task<Response<IList<CourseProgress>>> FetchAllProgress(Request request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyCourses.IsValueCreated)
                await _lazyCourses.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return new(_courses);
    }

    public async Task<CourseProgress> FetchProgress(Request<Course> request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyCourses.IsValueCreated)
                await _lazyCourses.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return _courses.FirstOrDefault(x => x.CourseId.Equals(request.Value.Id)) ?? new CourseProgress
        {
            CourseId = request.Value.Id,
            TimesCompleted = 0,
        };
    }

    public async Task<Response> AddCompleted(Request<CourseCompleted> request) =>
        await _proxyWrappers.Send("CourseRunAddCompleted", request);

    public async Task Handle(CourseProgressUpdated payload)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyCourses.IsValueCreated)
                await _lazyCourses.Value;
            else
            {
                lock (_coursesLock)
                {
                    _courses.RemoveAll(x => x.CourseId.Equals(payload.Progress.CourseId));
                    _courses.Add(payload.Progress);
                }
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task Handle(Reconnected payload)
    {
        await _initLock.WaitAsync();

        try
        {
            await FetchCoursesFromServer();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task FetchCoursesFromServer()
    {
        var response = await _proxyWrappers.Send<IList<CourseProgress>>("CourseRunFetchAllProgress", new());

        if (response.Ok)
            lock (_coursesLock)
                _courses = new(response.Value);
    }
}