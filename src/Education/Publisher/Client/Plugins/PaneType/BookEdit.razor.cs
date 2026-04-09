using AchievementAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementAdded;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class BookEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IBookService BookService { get; set; } = null!;

    public BookEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, BookService);
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

public class BookEditModel : EditModel<Book>,
    IHandle<BookSaved>, IHandle<BookRemoved>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IBookService _bookService;

    public BookEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IBookService bookService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _bookService = bookService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(BookSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(BookRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(AchievementAdded payload) => await FetchAchievementNames();

    public async Task Handle(AchievementSaved payload) => await FetchAchievementNames();

    public async Task Handle(AchievementRemoved payload) => await FetchAchievementNames();

    public ObservableCollection<Named> AchievementNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> BookCategoryNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> BookSeasonNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchAchievementNames(),
            FetchBookCategoryNames(),
            FetchBookSeasonNames(),
            FetchContentStatusNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var book = new Book
            {
                Title = "New Book",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                Key = String.Empty,
                Author = String.Empty,
                Isbn = String.Empty,
                Lexile = String.Empty,
                SeasonId = BookSeasonNames.MinBy(x => x.Ordinal)?.Id,
            };

            SetBook(book);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _bookService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetBook(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _bookService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/book-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _bookService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchAchievementNames()
    {
        var response = await WithAlerts(() => _bookService.FetchAchievementNames(new()), false);
        if (response.Ok) AchievementNames = response.Value.ToObservable();
    }

    public async Task FetchBookCategoryNames()
    {
        var response = await WithAlerts(() => _bookService.FetchBookCategoryNames(new()), false);
        if (response.Ok) BookCategoryNames = response.Value.ToList();
    }

    public async Task FetchBookSeasonNames()
    {
        var response = await WithAlerts(() => _bookService.FetchBookSeasonNames(new()), false);
        if (response.Ok) BookSeasonNames = response.Value.ToList();
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _bookService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    private void SetBook(Book book)
    {
        Entity = book;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}