namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class PageListForBookPreface : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IBookService BookService { get; set; } = null!;

    public PageListForBookPrefaceModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, BookService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public Task<Response<Page?>> AddPage()
    {
        var book = Model.Entity!;

        return BookService.AddPage(new(new()
        {
            BookId = book.Id,
            Page = new()
            {
                TypeId = PageTypeIds.StackedSections,
                Title = "New Page",
                StatusId = Crudspa.Framework.Core.Shared.Contracts.Ids.ContentStatusIds.Draft,
                ShowNotebook = false,
                ShowGuide = false,
            },
        }));
    }
}

public class PageListForBookPrefaceModel(Guid? id, IBookService bookService) : EditModel<Book>(false)
{
    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => bookService.FetchPrefaceBinderId(new(new() { Id = id })));

        if (response.Ok)
            Entity = response.Value;
    }
}