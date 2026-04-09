namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class EmailTemplateEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IEmailTemplateService EmailTemplateService { get; set; } = null!;

    public EmailTemplateEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var membershipId = Path!.Id("membership");

        Model = new(Path, Id, IsNew, membershipId, EventBus, Navigator, EmailTemplateService);
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

public class EmailTemplateEditModel : EditModel<EmailTemplate>,
    IHandle<EmailTemplateSaved>, IHandle<EmailTemplateRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _membershipId;
    private readonly INavigator _navigator;
    private readonly IEmailTemplateService _emailTemplateService;

    public List<String> Tokens
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public EmailTemplateEditModel(String? path, Guid? id, Boolean isNew, Guid? membershipId,
        IEventBus eventBus,
        INavigator navigator,
        IEmailTemplateService emailTemplateService) : base(isNew)
    {
        _path = path;
        _id = id;
        _membershipId = membershipId;
        _navigator = navigator;
        _emailTemplateService = emailTemplateService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(EmailTemplateSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(EmailTemplateRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await FetchTokens();
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetEmailTemplate(new()
            {
                MembershipId = _membershipId,
                Title = "New Email Template",
                Subject = String.Empty,
                Body = String.Empty,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _emailTemplateService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetEmailTemplate(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _emailTemplateService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/email-template-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _emailTemplateService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchTokens()
    {
        var response = await WithAlerts(() => _emailTemplateService.FetchTokens(new(new() { Id = _membershipId })), false);
        if (response.Ok)
            Tokens = response.Value.Select(x => $"[{x.Key!}]").ToList();
    }

    private void SetEmailTemplate(EmailTemplate emailTemplate)
    {
        Entity = emailTemplate;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}