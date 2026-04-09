namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class TrifoldPageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private Guid? TrifoldId => Path.Id("trifold");

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ITrifoldService TrifoldService { get; set; } = null!;

    public TrifoldPageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, TrifoldService, TrifoldId);
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

public class TrifoldPageEditModel(
    String? path,
    Guid? id,
    Boolean isNew,
    IEventBus eventBus,
    INavigator navigator,
    IScrollService scrollService,
    ITrifoldService trifoldService,
    Guid? trifoldId)
    : PageEditModelBase(path, id, isNew, eventBus, navigator, scrollService)
{
    protected override async Task<Response<IList<Orderable>>> FetchStatuses() =>
        await trifoldService.FetchContentStatusNames(new());

    protected override async Task<Response<Page?>> FetchPage(Guid? id) =>
        await trifoldService.FetchPage(new(new() { TrifoldId = trifoldId, Page = new() { Id = id } }));

    protected override async Task<Response<Page?>> AddPage(Page page) =>
        await trifoldService.AddPage(new(new() { TrifoldId = trifoldId, Page = page }));

    protected override async Task<Response> SavePage(Page page) =>
        await trifoldService.SavePage(new(new() { TrifoldId = trifoldId, Page = page }));
}