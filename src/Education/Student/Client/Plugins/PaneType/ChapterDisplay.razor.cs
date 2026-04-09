namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class ChapterDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public IChapterProgressService ChapterProgressService { get; set; } = null!;

    public ChapterDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, StudentAppService, ChapterProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ChapterDisplayModel : ScreenModel
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IStudentAppService _studentAppService;
    private readonly IChapterProgressService _chapterProgressService;
    private Chapter? _chapter;

    public ChapterDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IStudentAppService studentAppService,
        IChapterProgressService chapterProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _studentAppService = studentAppService;
        _chapterProgressService = chapterProgressService;

        eventBus.Subscribe(this);
    }

    public Chapter? Chapter
    {
        get => _chapter;
        set => SetProperty(ref _chapter, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchChapter(new(new() { Id = _id })));

        if (response.Ok)
            await SetChapter(response.Value);
    }

    public async Task HandleBinderCompleted()
    {
        await _chapterProgressService.AddCompleted(new(new()
        {
            ChapterId = _chapter!.Id,
            DeviceTimestamp = DateTimeOffset.Now,
        }));

        _navigator.Close(_path);
    }

    private Task SetChapter(Chapter chapter)
    {
        Chapter = chapter;

        _navigator.UpdateTitle(_path, Chapter.Title!);

        return Task.CompletedTask;
    }
}