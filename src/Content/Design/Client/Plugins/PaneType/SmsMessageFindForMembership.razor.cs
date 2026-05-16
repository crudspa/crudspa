namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SmsMessageFindForMembership : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISmsMessageService SmsMessageService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public SmsMessageFindForMembershipModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SmsMessageService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SmsMessageFindForMembershipModel : FindModel<SmsMessageSearch, SmsMessage>,
    IHandle<SmsMessageAdded>, IHandle<SmsMessageSaved>, IHandle<SmsMessageRemoved>
{
    private readonly ISmsMessageService _smsMessageService;
    private readonly Guid? _membershipId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public SmsMessageFindForMembershipModel(IEventBus eventBus, IScrollService scrollService, ISmsMessageService smsMessageService, Guid? membershipId)
        : base(scrollService)
    {
        _smsMessageService = smsMessageService;

        _membershipId = membershipId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Occurred",
            "Status",
        ];
    }

    public async Task Handle(SmsMessageAdded payload) => await Refresh();

    public async Task Handle(SmsMessageSaved payload) => await Refresh();

    public async Task Handle(SmsMessageRemoved payload) => await Refresh();

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _membershipId;

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
        var response = await WithWaiting("Searching...", () => _smsMessageService.SearchForMembership(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

}