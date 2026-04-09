namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ModulePageSections : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? ModuleId => Path.Id("module");
    private Guid? PageId => Path.Id("page") ?? Id;

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IModuleService ModuleService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ModulePageSectionsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ModuleService, ModuleId, PageId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ModulePageSectionsModel(
    IEventBus eventBus,
    IScrollService scrollService,
    IModuleService moduleService,
    Guid? moduleId,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await moduleService.FetchSections(new(new() { ModuleId = moduleId, PageId = pageId }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await moduleService.FetchSection(new(new() { ModuleId = moduleId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await moduleService.RemoveSection(new(new() { ModuleId = moduleId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await moduleService.SaveSectionOrder(new(new() { ModuleId = moduleId, PageId = sections.FirstOrDefault()?.PageId, Sections = sections }));
}