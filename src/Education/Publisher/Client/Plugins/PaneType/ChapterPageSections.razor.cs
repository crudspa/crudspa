namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ChapterPageSections : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? ChapterId => Path.Id("chapter");
    private Guid? PageId => Path.Id("page") ?? Id;

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IChapterService ChapterService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ChapterPageSectionsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ChapterService, ChapterId, PageId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ChapterPageSectionsModel(
    IEventBus eventBus,
    IScrollService scrollService,
    IChapterService chapterService,
    Guid? chapterId,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await chapterService.FetchSections(new(new() { ChapterId = chapterId, PageId = pageId }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await chapterService.FetchSection(new(new() { ChapterId = chapterId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await chapterService.RemoveSection(new(new() { ChapterId = chapterId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await chapterService.SaveSectionOrder(new(new() { ChapterId = chapterId, PageId = sections.FirstOrDefault()?.PageId, Sections = sections }));
}