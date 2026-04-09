namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class DistrictFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IDistrictService DistrictService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public DistrictFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, DistrictService);
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
        Navigator.GoTo($"{Path}/district-{Guid.NewGuid():D}?state=new");
    }
}

public class DistrictFindModel : FindModel<DistrictSearch, District>,
    IHandle<DistrictAdded>, IHandle<DistrictSaved>, IHandle<DistrictRemoved>,
    IHandle<DistrictContactAdded>, IHandle<DistrictContactRemoved>,
    IHandle<CommunityAdded>, IHandle<CommunityRemoved>,
    IHandle<SchoolAdded>, IHandle<SchoolRemoved>
{
    private readonly IDistrictService _districtService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public DistrictFindModel(IEventBus eventBus, IScrollService scrollService, IDistrictService districtService)
        : base(scrollService)
    {
        _districtService = districtService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Name",
        ];
    }

    public async Task Handle(DistrictAdded payload) => await Refresh();

    public async Task Handle(DistrictSaved payload) => await Refresh();

    public async Task Handle(DistrictRemoved payload) => await Refresh();

    public async Task Handle(DistrictContactAdded payload) => await Refresh();

    public async Task Handle(DistrictContactRemoved payload) => await Refresh();

    public async Task Handle(CommunityAdded payload) => await Refresh();

    public async Task Handle(CommunityRemoved payload) => await Refresh();

    public async Task Handle(SchoolAdded payload) => await Refresh();

    public async Task Handle(SchoolRemoved payload) => await Refresh();

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

        var request = new Request<DistrictSearch>(Search);
        var response = await WithWaiting("Searching...", () => _districtService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _districtService.Remove(new(new() { Id = id })));
    }
}