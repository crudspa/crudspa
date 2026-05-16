namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SmsTemplateFindForMembership : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISmsTemplateService SmsTemplateService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public SmsTemplateFindForMembershipModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var membershipId = Path!.Id("membership");
        var portalId = Path.Id("portal");

        Model = new(EventBus, ScrollService, SmsTemplateService, membershipId, portalId);
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
        Navigator.GoTo($"{Path}/sms-template-{Guid.NewGuid():D}?state=new");
    }
}

public class SmsTemplateFindForMembershipModel : FindModel<SmsTemplateSearch, SmsTemplate>,
    IHandle<SmsTemplateAdded>, IHandle<SmsTemplateSaved>, IHandle<SmsTemplateRemoved>
{
    private readonly ISmsTemplateService _smsTemplateService;
    private readonly Guid? _membershipId;
    private readonly Guid? _portalId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public SmsTemplateFindForMembershipModel(IEventBus eventBus, IScrollService scrollService, ISmsTemplateService smsTemplateService, Guid? membershipId, Guid? portalId)
        : base(scrollService)
    {
        _smsTemplateService = smsTemplateService;

        _membershipId = membershipId;
        _portalId = portalId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Title",
        ];
    }

    public async Task Handle(SmsTemplateAdded payload) => await Refresh();

    public async Task Handle(SmsTemplateSaved payload) => await Refresh();

    public async Task Handle(SmsTemplateRemoved payload) => await Refresh();

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
        Search.Sort.Ascending = true;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<SmsTemplateSearch>(Search);
        var response = await WithWaiting("Searching...", () => _membershipId is not null
            ? _smsTemplateService.SearchForMembership(request)
            : _smsTemplateService.SearchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _smsTemplateService.Remove(new(new() { Id = id })));
    }
}