using Crudspa.Education.Student.Client.Contracts.Events;

namespace Crudspa.Education.Student.Client.Services;

public class BookProgressServiceTcp : IBookProgressService, IHandle<BookProgressUpdated>, IHandle<Reconnected>
{
    private readonly IProxyWrappers _proxyWrappers;
    private readonly IEventBus _eventBus;
    private readonly Lazy<Task> _lazyBooks;
    private readonly Object _booksLock = new();
    private readonly SemaphoreSlim _initLock = new(1);
    private List<BookProgress> _books = [];

    public BookProgressServiceTcp(IProxyWrappers proxyWrappers, IEventBus eventBus)
    {
        _proxyWrappers = proxyWrappers;
        _eventBus = eventBus;
        _lazyBooks = new(FetchBooksFromServer);

        eventBus.Subscribe(this);
    }

    public async Task<Response<IList<BookProgress>>> FetchAll(Request request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyBooks.IsValueCreated)
                await _lazyBooks.Value;
        }
        finally
        {
            _initLock.Release();
        }

        return new(_books);
    }

    public async Task<BookProgress> Fetch(Request<Book> request)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyBooks.IsValueCreated)
                await _lazyBooks.Value;
        }
        finally
        {
            _initLock.Release();
        }

        var bookId = request.Value.Id;

        return _books.FirstOrDefault(x => x.BookId.Equals(bookId)) ?? new BookProgress
        {
            BookId = bookId,
            PrefaceCompletedCount = 0,
            ContentCompletedCount = 0,
            MapCompletedCount = 0,
        };
    }

    public async Task<Response> AddPrefaceCompleted(Request<PrefaceCompleted> request) =>
        await _proxyWrappers.Send("BookProgressAddPrefaceCompleted", request);

    public async Task Handle(BookProgressUpdated payload)
    {
        await _initLock.WaitAsync();

        try
        {
            if (!_lazyBooks.IsValueCreated)
                await _lazyBooks.Value;
            else
            {
                var existing = _books.FirstOrDefault(x => x.BookId.Equals(payload.Progress.BookId));

                var prefaceCompleted = payload.Progress.PrefaceCompletedCount > 0
                    && (existing is null || existing.PrefaceCompletedCount == 0);

                if (prefaceCompleted)
                {
                    await _eventBus.Publish(new MadeProgress
                    {
                        Title = "Nice Job!",
                        ImageUrl = "/api/education/student/images/star-filled",
                        Description = "You earned a star for finishing the intro!",
                    });
                }

                var contentCompleted = payload.Progress.ContentCompletedCount > 0
                    && (existing is null || existing.ContentCompletedCount == 0);

                if (contentCompleted)
                {
                    await _eventBus.Publish(new MadeProgress
                    {
                        Title = "Nice Reading!",
                        ImageUrl = "/api/education/student/images/star-filled",
                        Description = "You earned a star for reading all of the chapters!",
                    });
                }

                var mapCompleted = payload.Progress.MapCompletedCount > 0
                    && (existing is null || existing.MapCompletedCount == 0);

                if (mapCompleted)
                {
                    await _eventBus.Publish(new MadeProgress
                    {
                        Title = "Good Job!",
                        ImageUrl = "/api/education/student/images/star-filled",
                        Description = "You earned a star for exploring the map!",
                    });
                }

                lock (_booksLock)
                {
                    _books.RemoveAll(x => x.BookId.Equals(payload.Progress.BookId));
                    _books.Add(payload.Progress);
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
            await FetchBooksFromServer();
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task FetchBooksFromServer()
    {
        var response = await _proxyWrappers.Send<IList<BookProgress>>("BookProgressFetchAll", new());

        if (response.Ok)
            lock (_booksLock)
                _books = new(response.Value);
    }
}