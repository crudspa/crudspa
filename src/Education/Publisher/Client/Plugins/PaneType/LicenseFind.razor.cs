using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class LicenseFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ILicenseService LicenseService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public LicenseFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, LicenseService);
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
        Navigator.GoTo($"{Path}/license-{Guid.NewGuid():D}?state=new");
    }
}

public class LicenseFindModel : FindModel<LicenseSearch, License>,
    IHandle<LicenseAdded>, IHandle<LicenseSaved>, IHandle<LicenseRemoved>,
    IHandle<DistrictLicenseAdded>, IHandle<DistrictLicenseRemoved>,
    IHandle<UnitLicenseAdded>, IHandle<UnitLicenseRemoved>
{
    private readonly ILicenseService _licenseService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public LicenseFindModel(IEventBus eventBus, IScrollService scrollService, ILicenseService licenseService)
        : base(scrollService)
    {
        _licenseService = licenseService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Name",
        ];
    }

    public async Task Handle(LicenseAdded payload) => await Refresh();
    public async Task Handle(LicenseSaved payload) => await Refresh();
    public async Task Handle(LicenseRemoved payload) => await Refresh();
    public async Task Handle(DistrictLicenseAdded payload) => await Refresh();
    public async Task Handle(DistrictLicenseRemoved payload) => await Refresh();
    public async Task Handle(UnitLicenseAdded payload) => await Refresh();
    public async Task Handle(UnitLicenseRemoved payload) => await Refresh();

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

        var request = new Request<LicenseSearch>(Search);
        var response = await WithWaiting("Searching...", () => _licenseService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _licenseService.Remove(new(new() { Id = id })));
    }
}