namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class BookPrefaceSections : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? BookId => Path.Id("book");
    private Guid? PageId => Path.Id("page") ?? Id;

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IBookService BookService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public BookPrefaceSectionsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, BookService, BookId, PageId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class BookPrefaceSectionsModel(
    IEventBus eventBus,
    IScrollService scrollService,
    IBookService bookService,
    Guid? bookId,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await bookService.FetchSections(new(new() { BookId = bookId, PageId = pageId }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await bookService.FetchSection(new(new() { BookId = bookId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await bookService.RemoveSection(new(new() { BookId = bookId, PageId = section.PageId, Section = section }));

    protected override async Task<Response<Section?>> DuplicateSection(Section section) =>
        await bookService.DuplicateSection(new(new() { BookId = bookId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await bookService.SaveSectionOrder(new(new() { BookId = bookId, PageId = sections.FirstOrDefault()?.PageId, Sections = sections }));
}