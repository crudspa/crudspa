namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ChapterListForBook : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IChapterService ChapterService { get; set; } = null!;

    public ChapterListForBookModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ChapterService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ChapterListForBookModel : ListOrderablesModel<ChapterModel>,
    IHandle<ChapterAdded>, IHandle<ChapterSaved>, IHandle<ChapterRemoved>, IHandle<ChaptersReordered>,
    IHandle<PageAdded>, IHandle<PageRemoved>
{
    private readonly IChapterService _chapterService;
    private readonly Guid? _bookId;

    public ChapterListForBookModel(IEventBus eventBus, IScrollService scrollService, IChapterService chapterService, Guid? bookId)
        : base(scrollService)
    {
        _chapterService = chapterService;

        _bookId = bookId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ChapterAdded payload) => await Replace(payload.Id, payload.BookId);

    public async Task Handle(ChapterSaved payload) => await Replace(payload.Id, payload.BookId);

    public async Task Handle(ChapterRemoved payload) => await Rid(payload.Id, payload.BookId);

    public async Task Handle(ChaptersReordered payload) => await Refresh();

    public async Task Handle(PageAdded payload) => await Refresh(false);

    public async Task Handle(PageRemoved payload) => await Refresh(false);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Book>(new() { Id = _bookId });
        var response = await WithWaiting("Fetching...", () => _chapterService.FetchForBook(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ChapterModel(x)).ToList());
    }

    public override async Task<Response<ChapterModel?>> Fetch(Guid? id)
    {
        var response = await _chapterService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ChapterModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _chapterService.Remove(new(new()
        {
            Id = id,
            BookId = _bookId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_bookId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Chapter).ToList();
        return await WithWaiting("Saving...", () => _chapterService.SaveOrder(new(orderables)));
    }
}

public class ChapterModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleChapterChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Chapter));

    private Chapter _chapter;

    public String? Name => Chapter.Title;

    public ChapterModel(Chapter chapter)
    {
        _chapter = chapter;
        _chapter.PropertyChanged += HandleChapterChanged;
    }

    public void Dispose()
    {
        _chapter.PropertyChanged -= HandleChapterChanged;
    }

    public Guid? Id
    {
        get => _chapter.Id;
        set => _chapter.Id = value;
    }

    public Int32? Ordinal
    {
        get => _chapter.Ordinal;
        set => _chapter.Ordinal = value;
    }

    public Chapter Chapter
    {
        get => _chapter;
        set => SetProperty(ref _chapter, value);
    }
}