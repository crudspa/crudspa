namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SmsTemplateEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISmsTemplateService SmsTemplateService { get; set; } = null!;
    [Inject] public IMembershipService MembershipService { get; set; } = null!;
    [Inject] public ITokenService TokenService { get; set; } = null!;

    public SmsTemplateEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var membershipId = Path!.Id("membership");
        var portalId = Path.Id("portal");

        Model = new(Path, Id, IsNew, membershipId, portalId, EventBus, Navigator, SmsTemplateService, MembershipService, TokenService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class SmsTemplateEditModel : EditModel<SmsTemplate>,
    IHandle<SmsTemplateSaved>, IHandle<SmsTemplateRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _membershipId;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly ISmsTemplateService _smsTemplateService;
    private readonly IMembershipService _membershipService;
    private readonly ITokenService _tokenService;

    public SmsTemplateEditModel(String? path, Guid? id, Boolean isNew, Guid? membershipId, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        ISmsTemplateService smsTemplateService,
        IMembershipService membershipService,
        ITokenService tokenService) : base(isNew)
    {
        _path = path;
        _id = id;
        _membershipId = membershipId;
        _portalId = portalId;
        _navigator = navigator;
        _smsTemplateService = smsTemplateService;
        _membershipService = membershipService;
        _tokenService = tokenService;

        eventBus.Subscribe(this);
    }

    public ObservableCollection<Membership> Memberships
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Boolean ShowMembership => _membershipId is null;

    public List<String> Tokens
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Guid? SelectedMembershipId
    {
        get => Entity?.MembershipId;
        set
        {
            if (Entity is not null)
                Entity.MembershipId = value;
        }
    }

    public async Task Handle(SmsTemplateSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(SmsTemplateRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await FetchMemberships();
        await Refresh();
        await FetchTokens();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var smsTemplate = new SmsTemplate
            {
                MembershipId = _membershipId ?? Memberships.FirstOrDefault()?.Id,
                Title = "New Sms Template",
                Body = String.Empty,
            };

            SetSmsTemplate(smsTemplate);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _smsTemplateService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetSmsTemplate(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _smsTemplateService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/sms-template-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _smsTemplateService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchMemberships()
    {
        if (_membershipId is not null || _portalId is null)
            return;

        var response = await WithAlerts(() => _membershipService.FetchForPortal(new(new() { Id = _portalId })), false);
        if (response.Ok)
            Memberships = response.Value.ToObservable();
    }

    public async Task FetchTokens()
    {
        var membershipId = Entity?.MembershipId ?? _membershipId;
        if (membershipId is null)
        {
            Tokens = [];
            return;
        }

        var response = await WithAlerts(() => _tokenService.FetchForMembership(new(new() { Id = membershipId })), false);
        if (response.Ok)
            Tokens = response.Value.Select(x => $"[{x.Key!}]").ToList();
    }

    public async Task HandleMembershipChanged(Guid? id)
    {
        SelectedMembershipId = id;
        await FetchTokens();
    }


    private void SetSmsTemplate(SmsTemplate smsTemplate)
    {
        Entity = smsTemplate;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}