namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ModulePageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? ModuleId => Path.Id("module");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IModuleService ModuleService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ModulePageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, ModuleService, ModuleId);
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

public class ModulePageEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    IModuleService moduleService,
    Guid? moduleId)
    : PageEditModelBase(path, id, isNew, eventBus, navigator, scrollService)
{
    protected override async Task<Response<IList<Orderable>>> FetchStatuses() =>
        await moduleService.FetchContentStatusNames(new());

    protected override async Task<Response<Page?>> FetchPage(Guid? id) =>
        await moduleService.FetchPage(new(new() { ModuleId = moduleId, Page = new() { Id = id } }));

    protected override async Task<Response<Page?>> AddPage(Page page) =>
        await moduleService.AddPage(new(new() { ModuleId = moduleId, Page = page }));

    protected override async Task<Response> SavePage(Page page) =>
        await moduleService.SavePage(new(new() { ModuleId = moduleId, Page = page }));
}