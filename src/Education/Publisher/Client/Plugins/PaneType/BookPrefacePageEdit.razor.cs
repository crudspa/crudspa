namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class BookPrefacePageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? BookId => Path.Id("book");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IBookService BookService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public BookPrefacePageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, BookService, BookId);
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

public class BookPrefacePageEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    IBookService bookService,
    Guid? bookId)
    : PageEditModelBase(path, id, isNew, eventBus, navigator, scrollService)
{
    protected override async Task<Response<IList<Orderable>>> FetchStatuses() =>
        await bookService.FetchContentStatusNames(new());

    protected override async Task<Response<Page?>> FetchPage(Guid? id) =>
        await bookService.FetchPage(new(new() { BookId = bookId, Page = new() { Id = id } }));

    protected override async Task<Response<Page?>> AddPage(Page page) =>
        await bookService.AddPage(new(new() { BookId = bookId, Page = page }));

    protected override async Task<Response> SavePage(Page page) =>
        await bookService.SavePage(new(new() { BookId = bookId, Page = page }));
}