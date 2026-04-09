namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class CoursePageSections : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? CourseId => Path.Id("course");
    private Guid? PageId => Path.Id("page") ?? Id;

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public ICourseService CourseService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public CoursePageSectionsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, CourseService, CourseId, PageId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CoursePageSectionsModel(
    IEventBus eventBus,
    IScrollService scrollService,
    ICourseService courseService,
    Guid? courseId,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await courseService.FetchSections(new(new()
        {
            CourseId = courseId,
            PageId = pageId,
        }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await courseService.FetchSection(new(new()
        {
            CourseId = courseId,
            PageId = section.PageId,
            Section = section,
        }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await courseService.RemoveSection(new(new()
        {
            CourseId = courseId,
            PageId = section.PageId,
            Section = section,
        }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await courseService.SaveSectionOrder(new(new()
        {
            CourseId = courseId,
            PageId = sections.FirstOrDefault()?.PageId,
            Sections = sections,
        }));
}