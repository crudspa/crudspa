namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class BookFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IBookService BookService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public BookFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, BookService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void AddNew()
    {
        Navigator.GoTo($"{Path}/book-{Guid.NewGuid():D}?state=new");
    }
}

public class BookFindModel : FindModel<BookSearch, Book>,
    IHandle<BookAdded>, IHandle<BookSaved>, IHandle<BookRemoved>,
    IHandle<ChapterAdded>, IHandle<ChapterRemoved>,
    IHandle<GameAdded>, IHandle<GameRemoved>,
    IHandle<ModuleAdded>, IHandle<ModuleRemoved>,
    IHandle<TrifoldAdded>, IHandle<TrifoldRemoved>
{
    private readonly IBookService _bookService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public BookFindModel(IEventBus eventBus, IScrollService scrollService, IBookService bookService)
        : base(scrollService)
    {
        _bookService = bookService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Title",
            "Key",
            "Author",
        ];
    }

    public async Task Handle(BookAdded payload) => await Refresh();

    public async Task Handle(BookSaved payload) => await Refresh();

    public async Task Handle(BookRemoved payload) => await Refresh();

    public async Task Handle(ChapterAdded payload) => await Refresh();

    public async Task Handle(ChapterRemoved payload) => await Refresh();

    public async Task Handle(GameAdded payload) => await Refresh();

    public async Task Handle(GameRemoved payload) => await Refresh();

    public async Task Handle(ModuleAdded payload) => await Refresh();

    public async Task Handle(ModuleRemoved payload) => await Refresh();

    public async Task Handle(TrifoldAdded payload) => await Refresh();

    public async Task Handle(TrifoldRemoved payload) => await Refresh();

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> BookSeasonNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> BookCategoryNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = true;
        Search.Status.Clear();
        Search.Seasons.Clear();
        Search.Categories.Clear();

        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchBookSeasonNames(),
            FetchBookCategoryNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<BookSearch>(Search);
        var response = await WithWaiting("Searching...", () => _bookService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _bookService.Remove(new(new() { Id = id })));
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _bookService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchBookSeasonNames()
    {
        var response = await WithAlerts(() => _bookService.FetchBookSeasonNames(new()), false);
        if (response.Ok) BookSeasonNames = response.Value.ToList();
    }

    public async Task FetchBookCategoryNames()
    {
        var response = await WithAlerts(() => _bookService.FetchBookCategoryNames(new()), false);
        if (response.Ok) BookCategoryNames = response.Value.ToList();
    }
}