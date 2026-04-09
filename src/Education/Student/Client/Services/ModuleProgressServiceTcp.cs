using Crudspa.Education.Student.Client.Contracts.Events;

namespace Crudspa.Education.Student.Client.Services;

public class ModuleProgressServiceTcp : IModuleProgressService, IHandle<ModuleProgressUpdated>
{
    private readonly IProxyWrappers _proxyWrappers;
    private readonly IEventBus _eventBus;
    private readonly Lazy<Task> _lazyModules;
    private readonly Object _modulesLock = new();
    private readonly SemaphoreSlim _initLock = new(1);
    private List<ModuleProgress> _modules = [];

    public ModuleProgressServiceTcp(IProxyWrappers proxyWrappers, IEventBus eventBus)
    {
        _proxyWrappers = proxyWrappers;
        _eventBus = eventBus;
        _lazyModules = new(FetchModulesFromServer);

        eventBus.Subscribe(this);
    }

    public async Task<Response<IList<ModuleProgress>>> FetchAll(Request request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyModules.IsValueCreated)
                await _lazyModules.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return new(_modules);
    }

    public async Task<ModuleProgress> Fetch(Request<Module> request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyModules.IsValueCreated)
                await _lazyModules.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return _modules.FirstOrDefault(x => x.ModuleId.Equals(request.Value.Id)) ?? new ModuleProgress
        {
            ModuleId = request.Value.Id,
            ModuleCompletedCount = 0,
        };
    }

    public async Task<Response> AddCompleted(Request<ModuleCompleted> request) =>
        await _proxyWrappers.Send("ModuleProgressAddCompleted", request);

    public async Task Handle(ModuleProgressUpdated payload)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyModules.IsValueCreated)
                await _lazyModules.Value;
            else
            {
                var existing = _modules.FirstOrDefault(x => x.ModuleId.Equals(payload.Progress.ModuleId));

                var moduleCompleted = payload.Progress.ModuleCompletedCount > 0
                    && (existing is null || existing.ModuleCompletedCount == 0);

                if (moduleCompleted)
                {
                    await _eventBus.Publish(new MadeProgress
                    {
                        Title = "Well Done!",
                        ImageUrl = "/api/education/student/images/star-filled",
                        Description = "You earned a star for completing this module!",
                    });
                }

                lock (_modulesLock)
                {
                    _modules.RemoveAll(x => x.ModuleId.Equals(payload.Progress.ModuleId));
                    _modules.Add(payload.Progress);
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
            await FetchModulesFromServer();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task FetchModulesFromServer()
    {
        var response = await _proxyWrappers.Send<IList<ModuleProgress>>("ModuleProgressFetchAll", new());

        if (response.Ok)
            lock (_modulesLock)
                _modules = new(response.Value);
    }
}