namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ChapterEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IChapterService ChapterService { get; set; } = null!;

    public ChapterEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var bookId = Path!.Id("book");

        Model = new(Path, Id, IsNew, bookId, EventBus, Navigator, ChapterService);
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

public class ChapterEditModel : EditModel<Chapter>,
    IHandle<ChapterSaved>, IHandle<ChapterRemoved>, IHandle<ChaptersReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _bookId;
    private readonly INavigator _navigator;
    private readonly IChapterService _chapterService;

    public ChapterEditModel(String? path, Guid? id, Boolean isNew, Guid? bookId,
        IEventBus eventBus,
        INavigator navigator,
        IChapterService chapterService) : base(isNew)
    {
        _path = path;
        _id = id;
        _bookId = bookId;
        _navigator = navigator;
        _chapterService = chapterService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ChapterSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ChapterRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(ChaptersReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _chapterService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title);
        }
    }

    public List<Orderable> BinderTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchBinderTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var chapter = new Chapter
            {
                BookId = _bookId,
                Title = "New Chapter",
                Binder = new()
                {
                    TypeId = BinderTypeNames.MinBy(x => x.Ordinal)?.Id,
                },
            };

            SetChapter(chapter);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _chapterService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetChapter(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _chapterService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/chapter-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _chapterService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchBinderTypeNames()
    {
        var response = await WithAlerts(() => _chapterService.FetchBinderTypeNames(new()), false);
        if (response.Ok) BinderTypeNames = response.Value.ToList();
    }

    private void SetChapter(Chapter chapter)
    {
        Entity = chapter;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}