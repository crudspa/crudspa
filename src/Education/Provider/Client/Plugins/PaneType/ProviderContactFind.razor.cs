namespace Crudspa.Education.Provider.Client.Plugins.PaneType;

public partial class ProviderContactFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IProviderContactService ProviderContactService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public ProviderContactFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ProviderContactService);
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
        Navigator.GoTo($"{Path}/provider-contact-{Guid.NewGuid():D}?state=new");
    }
}

public class ProviderContactFindModel : FindModel<ProviderContactSearch, ProviderContact>,
    IHandle<ProviderContactAdded>, IHandle<ProviderContactSaved>, IHandle<ProviderContactRemoved>
{
    private readonly IProviderContactService _providerContactService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public ProviderContactFindModel(IEventBus eventBus, IScrollService scrollService, IProviderContactService providerContactService)
        : base(scrollService)
    {
        _providerContactService = providerContactService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "First",
            "Last",
            "Username",
        ];
    }

    public async Task Handle(ProviderContactAdded payload) => await Refresh();

    public async Task Handle(ProviderContactSaved payload) => await Refresh();

    public async Task Handle(ProviderContactRemoved payload) => await Refresh();

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

        var request = new Request<ProviderContactSearch>(Search);
        var response = await WithWaiting("Searching...", () => _providerContactService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _providerContactService.Remove(new(new() { Id = id })));
    }
}