namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ObjectivePageSections : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? ObjectiveId => Path.Id("objective");
    private Guid? PageId => Path.Id("page") ?? Id;

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IObjectiveService ObjectiveService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ObjectivePageSectionsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ObjectiveService, ObjectiveId, PageId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ObjectivePageSectionsModel(
    IEventBus eventBus,
    IScrollService scrollService,
    IObjectiveService objectiveService,
    Guid? objectiveId,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await objectiveService.FetchSections(new(new() { ObjectiveId = objectiveId, PageId = pageId }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await objectiveService.FetchSection(new(new() { ObjectiveId = objectiveId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await objectiveService.RemoveSection(new(new() { ObjectiveId = objectiveId, PageId = section.PageId, Section = section }));

    protected override async Task<Response<Section?>> DuplicateSection(Section section) =>
        await objectiveService.DuplicateSection(new(new() { ObjectiveId = objectiveId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await objectiveService.SaveSectionOrder(new(new() { ObjectiveId = objectiveId, PageId = sections.FirstOrDefault()?.PageId, Sections = sections }));
}