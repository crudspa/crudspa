namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class MessageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IMessageService MessageService { get; set; } = null!;
    [Inject] public IStageService StageService { get; set; } = null!;
    [Inject] public ICampaignService CampaignService { get; set; } = null!;
    [Inject] public IPopulationService PopulationService { get; set; } = null!;
    [Inject] public IEmailTemplateService EmailTemplateService { get; set; } = null!;
    [Inject] public ISmsTemplateService SmsTemplateService { get; set; } = null!;

    public MessageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, Path!.Id("stage"), Navigator, MessageService, StageService,
            CampaignService, PopulationService, EmailTemplateService, SmsTemplateService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class MessageEditModel : EditModel<Message>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _stageId;
    private readonly INavigator _navigator;
    private readonly IMessageService _messageService;
    private readonly IStageService _stageService;
    private readonly ICampaignService _campaignService;
    private readonly IPopulationService _populationService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ISmsTemplateService _smsTemplateService;

    public enum Channels
    {
        Email,
        Sms,
    }

    public EmailTemplate? EmailTemplate
    {
        get;
        set => SetProperty(ref field, value);
    }

    public SmsTemplate? SmsTemplate
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Orderable> PopulationNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> EmailNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> SmsNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<String> Tokens
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Channels Channel => Entity?.MessageTypeId == MessageTypeIds.Sms ? Channels.Sms : Channels.Email;

    public List<Orderable> TemplateNames => Channel == Channels.Email ? EmailNames : SmsNames;

    public Guid? TemplateId => Channel == Channels.Email ? Entity?.EmailTemplateId : Entity?.SmsTemplateId;

    public MessageEditModel(String? path, Guid? id, Boolean isNew, Guid? stageId,
        INavigator navigator,
        IMessageService messageService,
        IStageService stageService,
        ICampaignService campaignService,
        IPopulationService populationService,
        IEmailTemplateService emailTemplateService,
        ISmsTemplateService smsTemplateService) : base(isNew)
    {
        _path = path;
        _id = id;
        _stageId = stageId;
        _navigator = navigator;
        _messageService = messageService;
        _stageService = stageService;
        _campaignService = campaignService;
        _populationService = populationService;
        _emailTemplateService = emailTemplateService;
        _smsTemplateService = smsTemplateService;
    }

    public async Task Initialize()
    {
        var stageResponse = await _stageService.Fetch(new(new() { Id = _stageId }));
        if (!stageResponse.Ok || stageResponse.Value is null)
            return;

        var campaignResponse = await _campaignService.Fetch(new(new() { Id = stageResponse.Value.CampaignId }));
        if (!campaignResponse.Ok || campaignResponse.Value?.PortalId is null)
            return;

        var portalId = campaignResponse.Value.PortalId;
        var populationsTask = _populationService.FetchForPortal(new(new() { Id = portalId }));
        var emailsTask = _emailTemplateService.SearchForPortal(new(new()
        {
            ParentId = portalId,
            Paged = new() { PageNumber = 1, PageSize = 1000 },
        }));
        var smsTask = _smsTemplateService.SearchForPortal(new(new()
        {
            ParentId = portalId,
            Paged = new() { PageNumber = 1, PageSize = 1000 },
        }));

        await Task.WhenAll(populationsTask, emailsTask, smsTask);

        var populationsResponse = await populationsTask;
        var emailsResponse = await emailsTask;
        var smsResponse = await smsTask;

        if (populationsResponse.Ok) PopulationNames = Names(populationsResponse.Value);
        if (emailsResponse.Ok) EmailNames = Names(emailsResponse.Value.Where(IsReusable));
        if (smsResponse.Ok) SmsNames = Names(smsResponse.Value);

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            await SetMessage(new()
            {
                StageId = _stageId,
                Name = "New Message",
                PopulationId = PopulationNames.FirstOrDefault()?.Id,
                MessageTypeId = MessageTypeIds.Email,
                EmailTemplateId = EmailNames.FirstOrDefault()?.Id,
            });

            return;
        }

        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _messageService.Fetch(new(new() { Id = _id })));
        if (response.Ok)
            await SetMessage(response.Value);
    }

    public async Task Cancel()
    {
        if (IsNew)
            _navigator.Close(_path);
        else
            await Refresh();
    }

    public async Task SetChannel(Channels channel)
    {
        if (Entity is null)
            return;

        Entity.MessageTypeId = channel == Channels.Email ? MessageTypeIds.Email : MessageTypeIds.Sms;
        Entity.EmailTemplateId = channel == Channels.Email ? EmailNames.FirstOrDefault()?.Id : null;
        Entity.SmsTemplateId = channel == Channels.Sms ? SmsNames.FirstOrDefault()?.Id : null;

        await FetchTemplate();
        RaiseDerivedProperties();
    }

    public async Task SetTemplate(Guid? value)
    {
        if (Entity is null)
            return;

        if (Channel == Channels.Email)
            Entity.EmailTemplateId = value;
        else
            Entity.SmsTemplateId = value;

        await FetchTemplate();
        RaiseDerivedProperties();
    }

    public async Task SetPopulation(Guid? value)
    {
        if (Entity is null)
            return;

        Entity.PopulationId = value;
        await FetchTokens();
    }

    public async Task Save()
    {
        Response templateResponse;

        if (Channel == Channels.Email)
        {
            if (EmailTemplate is null)
                return;

            templateResponse = await _emailTemplateService.Save(new(EmailTemplate));
        }
        else
        {
            if (SmsTemplate is null)
                return;

            templateResponse = await _smsTemplateService.Save(new(SmsTemplate));
        }

        if (!templateResponse.Ok)
            return;

        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _messageService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/message-{response.Value.Id:D}");
                _navigator.Close(_path);
            }

            return;
        }

        var saveResponse = await WithWaiting("Saving...", () => _messageService.Save(new(Entity!)));
        if (saveResponse.Ok)
            ReadOnly = true;
    }

    private async Task SetMessage(Message message)
    {
        Entity = message;
        await Task.WhenAll(FetchTemplate(), FetchTokens());
        _navigator.UpdateTitle(_path, Entity.Name);
        RaiseDerivedProperties();
    }

    private async Task FetchTokens()
    {
        Tokens = [];

        if (Entity?.PopulationId is null)
            return;

        var response = await _populationService.FetchTokens(new(new() { Id = Entity.PopulationId }));
        if (response.Ok)
            Tokens = response.Value.OrderBy(x => x.Ordinal).Select(x => $"[{x.Key}]").ToList();
    }

    private async Task FetchTemplate()
    {
        EmailTemplate = null;
        SmsTemplate = null;

        if (Channel == Channels.Email && Entity?.EmailTemplateId is not null)
        {
            var response = await _emailTemplateService.Fetch(new(new() { Id = Entity.EmailTemplateId }));
            if (response.Ok) EmailTemplate = response.Value;
        }
        else if (Entity?.SmsTemplateId is not null)
        {
            var response = await _smsTemplateService.Fetch(new(new() { Id = Entity.SmsTemplateId }));
            if (response.Ok) SmsTemplate = response.Value;
        }
    }

    private void RaiseDerivedProperties()
    {
        RaisePropertyChanged(nameof(Channel));
        RaisePropertyChanged(nameof(TemplateNames));
        RaisePropertyChanged(nameof(TemplateId));
    }

    private static List<Orderable> Names<T>(IEnumerable<T> values) where T : INamed
    {
        return values.Select((x, i) => new Orderable
        {
            Id = x.Id,
            Name = x.Name,
            Ordinal = i,
        }).ToList();
    }

    private static Boolean IsReusable(EmailTemplate template) =>
        template.MembershipId is null && template.OrganizationId is null;
}