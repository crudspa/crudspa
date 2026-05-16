namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SmsFindForMembership : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISmsService SmsService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public SmsFindForMembershipModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var membershipId = Path!.Id("membership");
        var portalId = Path.Id("portal");

        Model = new(EventBus, ScrollService, SmsService, membershipId, portalId);
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
        Navigator.GoTo($"{Path}/sms-{Guid.NewGuid():D}?state=new");
    }
}

public class SmsFindForMembershipModel : FindModel<SmsSearch, Sms>,
    IHandle<SmsAdded>, IHandle<SmsSaved>, IHandle<SmsRemoved>
{
    private readonly ISmsService _smsService;
    private readonly Guid? _membershipId;
    private readonly Guid? _portalId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public SmsFindForMembershipModel(IEventBus eventBus, IScrollService scrollService, ISmsService smsService, Guid? membershipId, Guid? portalId)
        : base(scrollService)
    {
        _smsService = smsService;

        _membershipId = membershipId;
        _portalId = portalId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Send",
            "Status",
        ];
    }

    public async Task Handle(SmsAdded payload) => await Refresh();

    public async Task Handle(SmsSaved payload) => await Refresh();

    public async Task Handle(SmsRemoved payload) => await Refresh();

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _membershipId ?? _portalId;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = false;
        Search.SendRange.Type = DateRange.Types.Any;
        Search.ProcessedRange.Type = DateRange.Types.Any;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<SmsSearch>(Search);
        var response = await WithWaiting("Searching...", () => _membershipId is not null
            ? _smsService.SearchForMembership(request)
            : _smsService.SearchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _smsService.Remove(new(new() { Id = id })));
    }
}