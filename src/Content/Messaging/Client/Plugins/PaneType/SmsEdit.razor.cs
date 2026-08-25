namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class SmsEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public ISmsService SmsService { get; set; } = null!;
    [Inject] public IMembershipService MembershipService { get; set; } = null!;
    [Inject] public ITokenService TokenService { get; set; } = null!;

    public SmsEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var membershipId = Path!.Id("membership");
        var portalId = Path.Id("portal") ?? SessionState.Session.PortalId;

        Model = new(Path, Id, IsNew, membershipId, portalId, EventBus, Navigator, ScrollService, SmsService, MembershipService, TokenService);
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

public class SmsEditModel : EditModel<Sms>,
    IHandle<SmsSaved>, IHandle<SmsRemoved>,
    IHandle<SmsTemplateAdded>, IHandle<SmsTemplateSaved>, IHandle<SmsTemplateRemoved>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(args.PropertyName);
    public BatchModel<SmsAttachment> SmsAttachmentsModel { get; } = new();
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _membershipId;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly ISmsService _smsService;
    private readonly IMembershipService _membershipService;
    private readonly ITokenService _tokenService;

    public ModalModel TemplateModalModel { get; }

    public SmsEditModel(String? path, Guid? id, Boolean isNew, Guid? membershipId, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService,
        ISmsService smsService,
        IMembershipService membershipService,
        ITokenService tokenService) : base(isNew)
    {
        _path = path;
        _id = id;
        _membershipId = membershipId;
        _portalId = portalId;
        _navigator = navigator;
        _smsService = smsService;
        _membershipService = membershipService;
        _tokenService = tokenService;

        SmsAttachmentsModel.PropertyChanged += HandleModelChanged;

        TemplateModalModel = new(scrollService);

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        SmsAttachmentsModel.PropertyChanged -= HandleModelChanged;

        base.Dispose();
    }

    public async Task Handle(SmsSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(SmsRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(SmsTemplateAdded payload) => await FetchSmsTemplates();

    public async Task Handle(SmsTemplateSaved payload) => await FetchSmsTemplates();

    public async Task Handle(SmsTemplateRemoved payload) => await FetchSmsTemplates();

    public ObservableCollection<SmsTemplateFull> SmsTemplates
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public SmsTemplateFull? SelectedTemplate
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SelectedTemplateId
    {
        get => SelectedTemplate?.Id;
        set => HandleTemplateChanged(value);
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

    public async Task Initialize()
    {
        await FetchMemberships();
        await Refresh();
        await WithMany("Initializing...",
            FetchSmsTemplates(),
            FetchTokens());
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var sms = new Sms
            {
                MembershipId = _membershipId ?? Memberships.FirstOrDefault()?.Id,
                MembershipName = Memberships.FirstOrDefault(x => x.Id.Equals(_membershipId))?.Name ?? Memberships.FirstOrDefault()?.Name,
                Body = String.Empty,
            };

            SetSms(sms);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _smsService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetSms(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _smsService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/sms-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _smsService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public void AddSmsAttachment()
    {
        SmsAttachmentsModel.Entities.Add(new()
        {
            Id = Guid.NewGuid(),
            SmsId = _id,
            Ordinal = SmsAttachmentsModel.Entities.Count,
        });
    }

    public async Task FetchSmsTemplates()
    {
        if (_portalId is null)
        {
            SmsTemplates = [];
            SelectedTemplate = null;
            return;
        }

        var response = await WithAlerts(() => _smsService.FetchSmsTemplates(new(new() { Id = _portalId })), false);
        if (response.Ok)
        {
            SmsTemplates = response.Value.ToObservable();

            SelectedTemplate = SmsTemplates.FirstOrDefault(x => x.Id.Equals(Entity?.TemplateId))
                ?? SmsTemplates.FirstOrDefault();
        }
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

    public async Task FetchMemberships()
    {
        if (_membershipId is not null || _portalId is null)
            return;

        var response = await WithAlerts(() => _membershipService.FetchForPortal(new(new() { Id = _portalId })), false);
        if (response.Ok)
            Memberships = response.Value.OrderBy(x => x.Name).ToObservable();
    }

    public async Task HandleMembershipChanged(Guid? id)
    {
        SelectedMembershipId = id;
        if (Entity is not null)
        {
            Entity.MembershipName = Memberships.FirstOrDefault(x => x.Id.Equals(id))?.Name;

            UpdateTitle();
        }

        await FetchTokens();
    }

    public void HandleTemplateChanged(Guid? id)
    {
        SelectedTemplate = SmsTemplates.FirstOrDefault(x => x.Id.Equals(id)) ?? SmsTemplates.FirstOrDefault();
    }

    public async Task LoadTemplate()
    {
        if (Entity is not null && SelectedTemplate is not null)
        {
            Entity.TemplateId = SelectedTemplate.Id;
            Entity.TemplateTitle = SelectedTemplate.Title;
            Entity.Body = SelectedTemplate.Body;
        }

        await TemplateModalModel.Hide();
    }

    private void SetSms(Sms sms)
    {
        Entity = sms;
        SmsAttachmentsModel.Entities = sms.SmsAttachments;
        UpdateTitle();

        SelectedTemplate = SmsTemplates.FirstOrDefault(x => x.Id.Equals(Entity.TemplateId))
            ?? SmsTemplates.FirstOrDefault();
    }

    private void UpdateTitle() => _navigator.UpdateTitle(_path, Entity?.Name);
}