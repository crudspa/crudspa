namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class CoursePageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? CourseId => Path.Id("course");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public ICourseService CourseService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public CoursePageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, CourseService, CourseId);
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

public class CoursePageEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    ICourseService courseService,
    Guid? courseId)
    : PageEditModelBase(path, id, isNew, eventBus, navigator, scrollService)
{
    protected override async Task<Response<IList<Orderable>>> FetchStatuses() =>
        await courseService.FetchContentStatusNames(new());

    protected override async Task<Response<Page?>> FetchPage(Guid? id) =>
        await courseService.FetchPage(new(new()
        {
            CourseId = courseId,
            Page = new() { Id = id },
        }));

    protected override async Task<Response<Page?>> AddPage(Page page) =>
        await courseService.AddPage(new(new()
        {
            CourseId = courseId,
            Page = page,
        }));

    protected override async Task<Response> SavePage(Page page) =>
        await courseService.SavePage(new(new()
        {
            CourseId = courseId,
            Page = page,
        }));
}