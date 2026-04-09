namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

using Catalog = Shared.Contracts.Data.Catalog;

public partial class CatalogEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ICatalogService CatalogService { get; set; } = null!;

    public CatalogEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, CatalogService);
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
        await Model.Refresh();
    }
}

public class CatalogEditModel : EditModel<Catalog>,
    IHandle<CatalogSaved>
{
    private readonly ICatalogService _catalogService;

    public List<Named> PermissionNames = [];

    public CatalogEditModel(IEventBus eventBus,
        ICatalogService catalogService) : base(false)
    {
        _catalogService = catalogService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CatalogSaved payload)
    {
        if (payload.Id.Equals(Entity?.Id))
            await Refresh();
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchPermissionNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _catalogService.Fetch(new()));

        if (response.Ok)
            SetCatalog(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _catalogService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _catalogService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetCatalog(Catalog catalog)
    {
        Entity = catalog;
    }
}