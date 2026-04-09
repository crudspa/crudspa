namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ChapterPageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? ChapterId => Path.Id("chapter");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IChapterService ChapterService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ChapterPageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, ChapterService, ChapterId);
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

public class ChapterPageEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    IChapterService chapterService,
    Guid? chapterId)
    : PageEditModelBase(path, id, isNew, eventBus, navigator, scrollService)
{
    protected override async Task<Response<IList<Orderable>>> FetchStatuses() =>
        await chapterService.FetchContentStatusNames(new());

    protected override async Task<Response<Page?>> FetchPage(Guid? id) =>
        await chapterService.FetchPage(new(new()
        {
            ChapterId = chapterId,
            Page = new() { Id = id },
        }));

    protected override async Task<Response<Page?>> AddPage(Page page) =>
        await chapterService.AddPage(new(new()
        {
            ChapterId = chapterId,
            Page = page,
        }));

    protected override async Task<Response> SavePage(Page page) =>
        await chapterService.SavePage(new(new()
        {
            ChapterId = chapterId,
            Page = page,
        }));
}