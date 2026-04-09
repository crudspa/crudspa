namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class CatalogContactFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ICatalogContactService CatalogContactService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public CatalogContactFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, CatalogContactService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void AddNew()
    {
        Navigator.GoTo($"{Path}/catalog-contact-{Guid.NewGuid():D}?state=new");
    }
}

public class CatalogContactFindModel : FindModel<CatalogContactSearch, CatalogContact>,
    IHandle<CatalogContactAdded>, IHandle<CatalogContactSaved>, IHandle<CatalogContactRemoved>
{
    private readonly ICatalogContactService _catalogContactService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public CatalogContactFindModel(IEventBus eventBus, IScrollService scrollService, ICatalogContactService catalogContactService)
        : base(scrollService)
    {
        _catalogContactService = catalogContactService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "First Name",
            "Last Name",
            "Username",
        ];
    }

    public async Task Handle(CatalogContactAdded payload) => await Refresh();

    public async Task Handle(CatalogContactSaved payload) => await Refresh();

    public async Task Handle(CatalogContactRemoved payload) => await Refresh();

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = true;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<CatalogContactSearch>(Search);
        var response = await WithWaiting("Searching...", () => _catalogContactService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _catalogContactService.Remove(new(new() { Id = id })));
    }
}