namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class BookRelations : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IBookService BookService { get; set; } = null!;

    public BookRelationsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, EventBus, BookService);
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
        await Model.Refresh();
    }
}

public class BookRelationsModel : EditModel<Book>, IHandle<BookRelationsSaved>
{
    private readonly Guid? _id;
    private readonly IBookService _bookService;

    public BookRelationsModel(Guid? id,
        IEventBus eventBus,
        IBookService bookService) : base(false)
    {
        _id = id;
        _bookService = bookService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(BookRelationsSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _bookService.FetchRelations(new(new() { Id = _id })));

        if (response.Ok)
            SetBook(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _bookService.SaveRelations(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private void SetBook(Book book)
    {
        Entity = book;
    }
}