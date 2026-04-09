namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class ContentDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public IChapterProgressService ChapterProgressService { get; set; } = null!;

    public ContentDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, SoundService, StudentAppService, ChapterProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ContentDisplayModel : ScreenModel, IHandle<ChapterProgressUpdated>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ISoundService _soundService;
    private readonly IStudentAppService _studentAppService;
    private readonly IChapterProgressService _chapterProgressService;

    public ContentDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        ISoundService soundService,
        IStudentAppService studentAppService,
        IChapterProgressService chapterProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _soundService = soundService;
        _studentAppService = studentAppService;
        _chapterProgressService = chapterProgressService;

        eventBus.Subscribe(this);
    }

    public BookContent? BookContent
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchChapters(new(new() { Id = _id })));

        if (response.Ok)
            await SetBookContent(response.Value);
    }

    public Task Handle(ChapterProgressUpdated payload)
    {
        if (BookContent is null)
            return Task.CompletedTask;

        foreach (var chapter in BookContent.Chapters)
        {
            if (chapter.Id.Equals(payload.Progress.ChapterId))
            {
                chapter.Progress = payload.Progress;
                RaisePropertyChanged(nameof(BookContent));
                break;
            }
        }

        return Task.CompletedTask;
    }

    public void GoToChapter(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/chapter-{id:D}");
    }

    private async Task SetBookContent(BookContent bookContent)
    {
        foreach (var chapter in bookContent.Chapters)
            chapter.Progress = await _chapterProgressService.Fetch(new(new() { Id = chapter.Id }));

        BookContent = bookContent;

        _navigator.UpdateTitle(_path, BookContent.Title!);
    }
}