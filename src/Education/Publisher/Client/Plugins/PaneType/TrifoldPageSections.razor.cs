namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class TrifoldPageSections : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? TrifoldId => Path.Id("trifold");
    private Guid? PageId => Path.Id("page") ?? Id;

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ITrifoldService TrifoldService { get; set; } = null!;

    public TrifoldPageSectionsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, TrifoldService, TrifoldId, PageId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class TrifoldPageSectionsModel(
    IEventBus eventBus,
    IScrollService scrollService,
    ITrifoldService trifoldService,
    Guid? trifoldId,
    Guid? pageId)
    : PageSectionsModelBase(eventBus, scrollService, pageId)
{
    protected override async Task<Response<IList<Section>>> FetchSections(Guid? pageId) =>
        await trifoldService.FetchSections(new(new() { TrifoldId = trifoldId, PageId = pageId }));

    protected override async Task<Response<Section?>> FetchSection(Section section) =>
        await trifoldService.FetchSection(new(new() { TrifoldId = trifoldId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> RemoveSection(Section section) =>
        await trifoldService.RemoveSection(new(new() { TrifoldId = trifoldId, PageId = section.PageId, Section = section }));

    protected override async Task<Response<Section?>> DuplicateSection(Section section) =>
        await trifoldService.DuplicateSection(new(new() { TrifoldId = trifoldId, PageId = section.PageId, Section = section }));

    protected override async Task<Response> SaveSectionOrder(IList<Section> sections) =>
        await trifoldService.SaveSectionOrder(new(new() { TrifoldId = trifoldId, PageId = sections.FirstOrDefault()?.PageId, Sections = sections }));
}