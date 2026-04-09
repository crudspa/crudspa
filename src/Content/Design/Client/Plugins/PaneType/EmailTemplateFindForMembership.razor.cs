namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class EmailTemplateFindForMembership : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IEmailTemplateService EmailTemplateService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public EmailTemplateFindForMembershipModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, EmailTemplateService, Id);
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
        Navigator.GoTo($"{Path}/email-template-{Guid.NewGuid():D}?state=new");
    }
}

public class EmailTemplateFindForMembershipModel : FindModel<EmailTemplateSearch, EmailTemplate>,
    IHandle<EmailTemplateAdded>, IHandle<EmailTemplateSaved>, IHandle<EmailTemplateRemoved>
{
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly Guid? _membershipId;
    private Boolean _resetting;

    public EmailTemplateFindForMembershipModel(IEventBus eventBus, IScrollService scrollService, IEmailTemplateService emailTemplateService, Guid? membershipId)
        : base(scrollService)
    {
        _emailTemplateService = emailTemplateService;
        _membershipId = membershipId;
        eventBus.Subscribe(this);
    }

    public async Task Handle(EmailTemplateAdded payload) => await Refresh();

    public async Task Handle(EmailTemplateSaved payload) => await Refresh();

    public async Task Handle(EmailTemplateRemoved payload) => await Refresh();

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _membershipId;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<EmailTemplateSearch>(Search);
        var response = await WithWaiting("Searching...", () => _emailTemplateService.SearchForMembership(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _emailTemplateService.Remove(new(new() { Id = id })));
    }
}