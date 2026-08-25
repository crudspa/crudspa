namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class SmsTemplateEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public ISmsTemplateService SmsTemplateService { get; set; } = null!;
    [Inject] public ITokenService TokenService { get; set; } = null!;

    public SmsTemplateEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal") ?? SessionState.Session.PortalId;

        Model = new(Path, Id, IsNew, portalId, EventBus, Navigator, SmsTemplateService, TokenService);
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
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly ISmsTemplateService _smsTemplateService;
    private readonly ITokenService _tokenService;

    public SmsTemplateEditModel(String? path, Guid? id, Boolean isNew, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        ISmsTemplateService smsTemplateService,
        ITokenService tokenService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _navigator = navigator;
        _smsTemplateService = smsTemplateService;
        _tokenService = tokenService;

        eventBus.Subscribe(this);
    }

    public List<String> Tokens
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

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
                PortalId = _portalId,
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

    public async Task FetchTokens()
    {
        var portalId = Entity?.PortalId ?? _portalId;
        if (portalId is null)
        {
            Tokens = [];
            return;
        }

        var response = await WithAlerts(() => _tokenService.FetchForPortal(new(new() { Id = portalId })), false);
        if (response.Ok)
            Tokens = response.Value.Select(x => $"[{x.Key!}]").ToList();
    }

    private void SetSmsTemplate(SmsTemplate smsTemplate)
    {
        Entity = smsTemplate;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}