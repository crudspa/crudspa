namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ObjectivePageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? ObjectiveId => Path.Id("objective");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IObjectiveService ObjectiveService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ObjectivePageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, ObjectiveService, ObjectiveId);
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

public class ObjectivePageEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    IObjectiveService objectiveService,
    Guid? objectiveId)
    : PageEditModelBase(path, id, isNew, eventBus, navigator, scrollService)
{
    protected override async Task<Response<IList<Orderable>>> FetchStatuses() =>
        await objectiveService.FetchContentStatusNames(new());

    protected override async Task<Response<Page?>> FetchPage(Guid? id) =>
        await objectiveService.FetchPage(new(new() { ObjectiveId = objectiveId, Page = new() { Id = id } }));

    protected override async Task<Response<Page?>> AddPage(Page page) =>
        await objectiveService.AddPage(new(new() { ObjectiveId = objectiveId, Page = page }));

    protected override async Task<Response> SavePage(Page page) =>
        await objectiveService.SavePage(new(new() { ObjectiveId = objectiveId, Page = page }));
}