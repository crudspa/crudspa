namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class ShirtFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IShirtService ShirtService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public ShirtFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ShirtService);
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
        Navigator.GoTo($"{Path}/shirt-{Guid.NewGuid():D}?state=new");
    }
}

public class ShirtFindModel : FindModel<ShirtSearch, Shirt>,
    IHandle<ShirtAdded>, IHandle<ShirtSaved>, IHandle<ShirtRemoved>,
    IHandle<ShirtOptionAdded>, IHandle<ShirtOptionRemoved>
{
    private readonly IShirtService _shirtService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public ShirtFindModel(IEventBus eventBus, IScrollService scrollService, IShirtService shirtService)
        : base(scrollService)
    {
        _shirtService = shirtService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Name",
            "Price",
        ];
    }

    public async Task Handle(ShirtAdded payload) => await Refresh();

    public async Task Handle(ShirtSaved payload) => await Refresh();

    public async Task Handle(ShirtRemoved payload) => await Refresh();

    public async Task Handle(ShirtOptionAdded payload) => await Refresh();

    public async Task Handle(ShirtOptionRemoved payload) => await Refresh();

    public List<Orderable> BrandNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

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
        Search.Brands.Clear();

        await WithMany("Initializing...",
            FetchBrandNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<ShirtSearch>(Search);
        var response = await WithWaiting("Searching...", () => _shirtService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _shirtService.Remove(new(new() { Id = id })));
    }

    public async Task FetchBrandNames()
    {
        var response = await WithAlerts(() => _shirtService.FetchBrandNames(new()), false);
        if (response.Ok) BrandNames = response.Value.ToList();
    }
}