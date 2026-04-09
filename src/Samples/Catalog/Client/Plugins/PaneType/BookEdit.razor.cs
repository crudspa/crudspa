namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

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
    IHandle<BookSaved>, IHandle<BookRemoved>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(args.PropertyName);
    public BatchModel<BookEdition> BookEditionsModel { get; } = new();
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

        BookEditionsModel.PropertyChanged += HandleModelChanged;

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        BookEditionsModel.PropertyChanged -= HandleModelChanged;

        base.Dispose();
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

    public List<Orderable> FormatNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> GenreNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> TagNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchFormatNames(),
            FetchGenreNames(),
            FetchTagNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var book = new Book
            {
                Isbn = String.Empty,
                Title = "New Book",
                Author = String.Empty,
                GenreId = GenreNames.MinBy(x => x.Ordinal)?.Id,
            };

        foreach (var tag in TagNames)
            book.Tags.Add(new()
            {
                Id = tag.Id,
                Name = tag.Name,
                Selected = false,
            });

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

    public void AddBookEdition()
    {
        BookEditionsModel.Entities.Add(new()
        {
            Id = Guid.NewGuid(),
            BookId = _id,
            Ordinal = BookEditionsModel.Entities.Count,
        });
    }

    public async Task FetchFormatNames()
    {
        var response = await WithAlerts(() => _bookService.FetchFormatNames(new()), false);
        if (response.Ok) FormatNames = response.Value.ToList();
    }

    public async Task FetchGenreNames()
    {
        var response = await WithAlerts(() => _bookService.FetchGenreNames(new()), false);
        if (response.Ok) GenreNames = response.Value.ToList();
    }

    public async Task FetchTagNames()
    {
        var response = await WithAlerts(() => _bookService.FetchTagNames(new()), false);
        if (response.Ok) TagNames = response.Value.ToList();
    }

    private void SetBook(Book book)
    {
        Entity = book;
        BookEditionsModel.Entities = book.BookEditions;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}