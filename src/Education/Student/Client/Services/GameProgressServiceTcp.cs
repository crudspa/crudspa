using Crudspa.Education.Student.Client.Contracts.Events;

namespace Crudspa.Education.Student.Client.Services;

public class GameProgressServiceTcp : IGameProgressService, IHandle<GameProgressUpdated>
{
    private readonly IProxyWrappers _proxyWrappers;
    private readonly IEventBus _eventBus;
    private readonly Lazy<Task> _lazyGames;
    private readonly Object _gamesLock = new();
    private readonly SemaphoreSlim _initLock = new(1);
    private List<GameProgress> _games = [];

    public GameProgressServiceTcp(IProxyWrappers proxyWrappers, IEventBus eventBus)
    {
        _proxyWrappers = proxyWrappers;
        _eventBus = eventBus;
        _lazyGames = new(FetchGamesFromServer);

        eventBus.Subscribe(this);
    }

    public async Task<Response<IList<GameProgress>>> FetchAll(Request request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyGames.IsValueCreated)
                await _lazyGames.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return new(_games);
    }

    public async Task<GameProgress> Fetch(Request<Game> request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyGames.IsValueCreated)
                await _lazyGames.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return _games.FirstOrDefault(x => x.GameId.Equals(request.Value.Id)) ?? new GameProgress
        {
            GameId = request.Value.Id,
            GameCompletedCount = 0,
        };
    }

    public async Task<Response> AddCompleted(Request<GameCompleted> request) =>
        await _proxyWrappers.Send("GameProgressAddCompleted", request);

    public async Task Handle(GameProgressUpdated payload)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyGames.IsValueCreated)
                await _lazyGames.Value;
            else
            {
                var existing = _games.FirstOrDefault(x => x.GameId.Equals(payload.Progress.GameId));

                var gameCompleted = payload.Progress.GameCompletedCount > 0
                    && (existing is null || existing.GameCompletedCount == 0);

                if (gameCompleted)
                {
                    await _eventBus.Publish(new MadeProgress
                    {
                        Title = "You're Amazing!",
                        ImageUrl = "/api/education/student/images/star-filled",
                        Description = "You earned a star for completing a game!",
                    });
                }

                lock (_gamesLock)
                {
                    _games.RemoveAll(x => x.GameId.Equals(payload.Progress.GameId));
                    _games.Add(payload.Progress);
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
            await FetchGamesFromServer();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task FetchGamesFromServer()
    {
        var response = await _proxyWrappers.Send<IList<GameProgress>>("GameProgressFetchAll", new());

        if (response.Ok)
            lock (_gamesLock)
                _games = new(response.Value);
    }
}