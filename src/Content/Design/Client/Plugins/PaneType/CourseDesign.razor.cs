namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class CourseDesign : IPaneDesign, IHasPaneId, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }

    [Parameter] public String? Path { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? PaneId { get; set; }
    [Parameter] public EventCallback ConfigUpdated { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IContentPortalService ContentPortalService { get; set; } = null!;
    [Inject] public IPanePageService PanePageService { get; set; } = null!;

    public CourseDesignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal");

        Model = new(PaneId, ContentPortalService, PanePageService, portalId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public String? GetConfigJson() => null;

    public async Task<Boolean> PrepareForSave() => (await Model.Save()).Ok;
}

public class CourseDesignModel(Guid? paneId, IContentPortalService contentPortalService, IPanePageService panePageService, Guid? portalId) : ScreenModel
{
    public CourseConfig.IdSources IdSource
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 CourseIdSource => IdSource == CourseConfig.IdSources.SpecificCourse ? 1 : 0;

    public Guid? CourseId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Named> Courses
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        var coursePaneResponse = await WithWaiting("Fetching...", () =>
            panePageService.FetchCoursePane(new(new() { PaneId = paneId })));

        if (coursePaneResponse.Ok && coursePaneResponse.Value is not null)
        {
            IdSource = coursePaneResponse.Value.IdSource == 1 ? CourseConfig.IdSources.SpecificCourse : CourseConfig.IdSources.FromUrl;
            CourseId = coursePaneResponse.Value.CourseId;
        }

        var request = new Request<ContentPortal>(new() { Id = portalId });
        var response = await WithWaiting("Fetching...", () => contentPortalService.FetchCourseNames(request));

        if (response.Ok)
        {
            Courses = response.Value.ToObservable();
            if (IdSource == CourseConfig.IdSources.SpecificCourse)
                CourseId ??= Courses.FirstOrDefault()?.Id;
        }
    }

    public async Task<Response> Save()
    {
        return await WithWaiting("Saving...", () => panePageService.SaveCoursePane(new(new()
        {
            PaneId = paneId,
            IdSource = CourseIdSource,
            CourseId = IdSource == CourseConfig.IdSources.SpecificCourse ? CourseId : null,
        })));
    }
}