namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class EmailFindForMembership : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IEmailService EmailService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public EmailFindForMembershipModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, EmailService, Id);
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
        Navigator.GoTo($"{Path}/email-{Guid.NewGuid():D}?state=new");
    }
}

public class EmailFindForMembershipModel : FindModel<EmailSearch, Email>,
    IHandle<EmailAdded>, IHandle<EmailSaved>, IHandle<EmailRemoved>
{
    private readonly IEmailService _emailService;
    private readonly Guid? _membershipId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public EmailFindForMembershipModel(IEventBus eventBus, IScrollService scrollService, IEmailService emailService, Guid? membershipId)
        : base(scrollService)
    {
        _emailService = emailService;
        _membershipId = membershipId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Send",
            "Subject",
        ];
    }

    public async Task Handle(EmailAdded payload) => await Refresh();

    public async Task Handle(EmailSaved payload) => await Refresh();

    public async Task Handle(EmailRemoved payload) => await Refresh();

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
        Search.SendRange.Type = DateRange.Types.Any;
        Search.ProcessedRange.Type = DateRange.Types.Any;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<EmailSearch>(Search);
        var response = await WithWaiting("Searching...", () => _emailService.SearchForMembership(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _emailService.Remove(new(new() { Id = id })));
    }
}