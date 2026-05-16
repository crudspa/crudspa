namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SmsEventFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISmsEventService SmsEventService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public SmsEventFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SmsEventService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SmsEventFindModel : FindModel<SmsEventSearch, SmsEvent>,
    IHandle<SmsEventAdded>, IHandle<SmsEventSaved>, IHandle<SmsEventRemoved>
{
    private readonly ISmsEventService _smsEventService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public SmsEventFindModel(IEventBus eventBus, IScrollService scrollService, ISmsEventService smsEventService)
        : base(scrollService)
    {
        _smsEventService = smsEventService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Received",
            "Status",
        ];
    }

    public async Task Handle(SmsEventAdded payload) => await Refresh();

    public async Task Handle(SmsEventSaved payload) => await Refresh();

    public async Task Handle(SmsEventRemoved payload) => await Refresh();

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
        Search.Sort.Ascending = false;
        Search.ReceivedRange.Type = DateRange.Types.Any;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<SmsEventSearch>(Search);
        var response = await WithWaiting("Searching...", () => _smsEventService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await Task.CompletedTask;
    }
}