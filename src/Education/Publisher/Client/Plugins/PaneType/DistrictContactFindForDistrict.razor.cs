namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class DistrictContactFindForDistrict : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IDistrictContactService DistrictContactService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public DistrictContactFindForDistrictModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, DistrictContactService, Id);
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
        Navigator.GoTo($"{Path}/district-contact-{Guid.NewGuid():D}?state=new");
    }
}

public class DistrictContactFindForDistrictModel : FindModel<DistrictContactSearch, DistrictContact>,
    IHandle<DistrictContactAdded>, IHandle<DistrictContactSaved>, IHandle<DistrictContactRemoved>
{
    private readonly IDistrictContactService _districtContactService;
    private readonly Guid? _districtId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public DistrictContactFindForDistrictModel(IEventBus eventBus, IScrollService scrollService, IDistrictContactService districtContactService, Guid? districtId)
        : base(scrollService)
    {
        _districtContactService = districtContactService;

        _districtId = districtId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "First",
            "Last",
            "Username",
        ];
    }

    public async Task Handle(DistrictContactAdded payload) => await Refresh();

    public async Task Handle(DistrictContactSaved payload) => await Refresh();

    public async Task Handle(DistrictContactRemoved payload) => await Refresh();

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _districtId;

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

        var request = new Request<DistrictContactSearch>(Search);
        var response = await WithWaiting("Searching...", () => _districtContactService.SearchForDistrict(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _districtContactService.Remove(new(new() { Id = id })));
    }
}