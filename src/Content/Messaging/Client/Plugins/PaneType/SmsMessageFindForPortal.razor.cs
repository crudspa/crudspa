namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class SmsMessageFindForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public ISmsMessageService SmsMessageService { get; set; } = null!;

    public SmsMessageFindForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal") ?? Id ?? SessionState.Session.PortalId;
        var config = ConfigJson.FromJson<SmsMessageFindForPortalConfig>() ?? new();

        Model = new(EventBus, ScrollService, SmsMessageService, portalId, config.IncludeAllDirections);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SmsMessageFindForPortalModel : FindModel<SmsMessageSearch, SmsMessage>,
    IHandle<SmsMessageAdded>, IHandle<SmsMessageSaved>, IHandle<SmsMessageRemoved>
{
    private readonly ISmsMessageService _smsMessageService;
    private readonly Guid? _portalId;
    private readonly Boolean _includeAllDirections;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public SmsMessageFindForPortalModel(IEventBus eventBus, IScrollService scrollService, ISmsMessageService smsMessageService, Guid? portalId, Boolean includeAllDirections)
        : base(scrollService)
    {
        _smsMessageService = smsMessageService;

        _portalId = portalId;
        _includeAllDirections = includeAllDirections;
        eventBus.Subscribe(this);

        _sorts = ["Occurred", "Status", "Direction"];
    }

    public async Task Handle(SmsMessageAdded payload) => await Refresh();

    public async Task Handle(SmsMessageSaved payload) => await Refresh();

    public async Task Handle(SmsMessageRemoved payload) => await Refresh();

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public Boolean IncludeAllDirections => _includeAllDirections;

    public String EmptyEntityName => IncludeAllDirections ? "deliveries" : "conversations";

    public String OccurredLabel => IncludeAllDirections ? "Occurred" : "Last Activity";

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _portalId;
        Search.Direction = null;
        Search.LatestForConversation = !IncludeAllDirections;
        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = false;
        Search.OccurredRange.Type = DateRange.Types.Any;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<SmsMessageSearch>(Search);
        var response = await WithWaiting("Searching...", () => _smsMessageService.SearchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }
}

public class SmsMessageFindForPortalConfig
{
    public Boolean IncludeAllDirections { get; set; }
}