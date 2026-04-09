using Crudspa.Education.Student.Client.Contracts.Events;

namespace Crudspa.Education.Student.Client.Services;

public class ObjectiveProgressServiceTcp : IObjectiveProgressService, IHandle<ObjectiveProgressUpdated>
{
    private readonly IProxyWrappers _proxyWrappers;
    private readonly IEventBus _eventBus;
    private readonly IStudentAppService _studentAppService;
    private readonly Lazy<Task> _lazyObjectives;
    private readonly Object _objectivesLock = new();
    private readonly SemaphoreSlim _initLock = new(1);
    private List<ObjectiveProgress> _objectives = [];

    public ObjectiveProgressServiceTcp(IProxyWrappers proxyWrappers, IEventBus eventBus, IStudentAppService studentAppService)
    {
        _proxyWrappers = proxyWrappers;
        _eventBus = eventBus;
        _studentAppService = studentAppService;
        _lazyObjectives = new(FetchObjectivesFromServer);

        eventBus.Subscribe(this);
    }

    public async Task<Response<IList<ObjectiveProgress>>> FetchAll(Request request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyObjectives.IsValueCreated)
                await _lazyObjectives.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return new(_objectives);
    }

    public async Task<ObjectiveProgress> Fetch(Request<Objective> request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyObjectives.IsValueCreated)
                await _lazyObjectives.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return _objectives.FirstOrDefault(x => x.ObjectiveId.Equals(request.Value.Id)) ?? new ObjectiveProgress
        {
            ObjectiveId = request.Value.Id,
            TimesCompleted = 0,
        };
    }

    public async Task<Response> AddCompleted(Request<ObjectiveCompleted> request) =>
        await _proxyWrappers.Send("ObjectiveProgressAddCompleted", request);

    public async Task Handle(ObjectiveProgressUpdated payload)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyObjectives.IsValueCreated)
                await _lazyObjectives.Value;
            else
            {
                var existing = _objectives.FirstOrDefault(x => x.ObjectiveId.Equals(payload.Progress.ObjectiveId));

                var objectiveCompleted = payload.Progress.TimesCompleted > 0
                    && (existing is null || existing.TimesCompleted == 0);

                if (objectiveCompleted)
                {
                    var response = await _studentAppService.FetchObjective(new(new() { Id = payload.Progress.ObjectiveId }));

                    if (response.Ok)
                    {
                        await _eventBus.Publish(new MadeProgress
                        {
                            Title = "Excellent Work!",
                            ImageUrl = $"/api/framework/core/image-file/fetch?id={response.Value.TrophyImageId:D}&width=160",
                            Description = "You completed an objective!",
                        });
                    }
                }

                lock (_objectivesLock)
                {
                    _objectives.RemoveAll(x => x.ObjectiveId.Equals(payload.Progress.ObjectiveId));
                    _objectives.Add(payload.Progress);
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
            await FetchObjectivesFromServer();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task FetchObjectivesFromServer()
    {
        var response = await _proxyWrappers.Send<IList<ObjectiveProgress>>("ObjectiveProgressFetchAll", new());

        if (response.Ok)
            lock (_objectivesLock)
                _objectives = new(response.Value);
    }
}