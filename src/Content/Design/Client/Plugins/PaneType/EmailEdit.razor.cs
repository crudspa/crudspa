namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class EmailEdit : IPaneDisplay, IDisposable
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
    [Inject] public IEmailService EmailService { get; set; } = null!;

    public EmailEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var membershipId = Path!.Id("membership");

        Model = new(Path, Id, IsNew, membershipId, EventBus, Navigator, ScrollService, SessionState, EmailService);
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

public class EmailEditModel : EditModel<Email>,
    IHandle<EmailSaved>, IHandle<EmailRemoved>,
    IHandle<EmailTemplateAdded>, IHandle<EmailTemplateSaved>, IHandle<EmailTemplateRemoved>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(args.PropertyName);

    public BatchModel<EmailAttachment> EmailAttachmentsModel { get; } = new();

    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _membershipId;
    private readonly INavigator _navigator;
    private readonly ISessionState _sessionState;
    private readonly IEmailService _emailService;

    public ModalModel TemplateModalModel { get; }

    public EmailEditModel(String? path, Guid? id, Boolean isNew, Guid? membershipId,
        IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService,
        ISessionState sessionState,
        IEmailService emailService) : base(isNew)
    {
        _path = path;
        _id = id;
        _membershipId = membershipId;
        _navigator = navigator;
        _sessionState = sessionState;
        _emailService = emailService;

        EmailAttachmentsModel.PropertyChanged += HandleModelChanged;

        TemplateModalModel = new(scrollService);

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        EmailAttachmentsModel.PropertyChanged -= HandleModelChanged;

        base.Dispose();
    }

    public async Task Handle(EmailSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(EmailRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(EmailTemplateAdded payload) => await FetchEmailTemplates();

    public async Task Handle(EmailTemplateSaved payload) => await FetchEmailTemplates();

    public async Task Handle(EmailTemplateRemoved payload) => await FetchEmailTemplates();

    public ObservableCollection<EmailTemplateFull> EmailTemplates
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public EmailTemplateFull? SelectedTemplate
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SelectedTemplateId
    {
        get => SelectedTemplate?.Id;
        set => HandleTemplateChanged(value);
    }

    public List<String> Tokens
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchEmailTemplates(),
            FetchTokens());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var localTime = DateTimeEx.ToLocalTime(DateTimeOffset.Now, _sessionState.TimeZoneId);

            SetEmail(new()
            {
                MembershipId = _membershipId,
                FromName = _sessionState.Session.User?.Name ?? String.Empty,
                FromEmail = _sessionState.Session.User?.Username ?? String.Empty,
                Subject = "New Email",
                Send = localTime.GetValueOrDefault().AddDays(1).Date.AddHours(8),
                Body = String.Empty,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _emailService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetEmail(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _emailService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/email-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _emailService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public void AddEmailAttachment()
    {
        EmailAttachmentsModel.Entities.Add(new()
        {
            Id = Guid.NewGuid(),
            EmailId = _id,
            Ordinal = EmailAttachmentsModel.Entities.Count,
        });
    }

    public async Task FetchEmailTemplates()
    {
        var response = await WithAlerts(() => _emailService.FetchEmailTemplates(new(new() { Id = _membershipId })), false);
        if (response.Ok)
        {
            EmailTemplates = response.Value.ToObservable();
            SelectedTemplate = EmailTemplates.FirstOrDefault(x => x.Id.Equals(Entity?.TemplateId)) ?? EmailTemplates.FirstOrDefault();
        }
    }

    public async Task FetchTokens()
    {
        var response = await WithAlerts(() => _emailService.FetchTokens(new(new() { Id = _membershipId })), false);
        if (response.Ok)
            Tokens = response.Value.Select(x => $"[{x.Key!}]").ToList();
    }

    public void HandleTemplateChanged(Guid? id)
    {
        SelectedTemplate = EmailTemplates.FirstOrDefault(x => x.Id.Equals(id)) ?? EmailTemplates.FirstOrDefault();
    }

    public async Task LoadTemplate()
    {
        if (Entity is not null && SelectedTemplate is not null)
        {
            Entity.TemplateId = SelectedTemplate.Id;
            Entity.Subject = SelectedTemplate.Subject;
            Entity.Body = SelectedTemplate.Body;
        }

        await TemplateModalModel.Hide();
    }

    private void SetEmail(Email email)
    {
        Entity = email;
        EmailAttachmentsModel.Entities = email.EmailAttachments;
        _navigator.UpdateTitle(_path, Entity.Subject);

        SelectedTemplate = EmailTemplates.FirstOrDefault(x => x.Id.Equals(Entity.TemplateId))
            ?? EmailTemplates.FirstOrDefault();
    }
}