namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class BookPrefaceSectionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? BookId => Path.Id("book");
    private Guid? PageId => Path.Id("page");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IBookService BookService { get; set; } = null!;
    [Inject] public IContainerService ContainerService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IItemService ItemService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISectionService SectionService { get; set; } = null!;

    public BookPrefaceSectionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, SectionService, ItemService, ContainerService, BookService, BookId, PageId);
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

public class BookPrefaceSectionEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    ISectionService sectionService,
    IItemService itemService,
    IContainerService containerService,
    IBookService bookService,
    Guid? bookId,
    Guid? pageId)
    : SectionEditModel(path, id, isNew, pageId, eventBus, navigator, scrollService, sectionService, itemService, containerService)
{
    protected override async Task<Response<Section?>> FetchSection(Guid? id) =>
        await bookService.FetchSection(new(new()
        {
            BookId = bookId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = new() { Id = id },
        }));

    protected override async Task<Response<Section?>> AddSection(Section section) =>
        await bookService.AddSection(new(new()
        {
            BookId = bookId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = section,
        }));

    protected override async Task<Response> SaveSection(Section section) =>
        await bookService.SaveSection(new(new()
        {
            BookId = bookId,
            PageId = Entity?.PageId ?? DefaultPageId,
            Section = section,
        }));
}