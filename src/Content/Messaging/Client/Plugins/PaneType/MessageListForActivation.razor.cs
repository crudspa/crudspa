namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class MessageListForActivation : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IMessageService MessageService { get; set; } = null!;
    [Inject] public IEmailService EmailService { get; set; } = null!;

    public MessageListForActivationModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, MessageService, EmailService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class MessageListForActivationModel : ListModel<MessageModel>,
    IHandle<MessageAdded>, IHandle<MessageSaved>, IHandle<MessageRemoved>,
    IHandle<EmailAdded>, IHandle<EmailSaved>, IHandle<EmailRemoved>
{
    private readonly IMessageService _messageService;
    private readonly IEmailService _emailService;
    private readonly Guid? _activationId;

    public MessageListForActivationModel(IEventBus eventBus, IScrollService scrollService,
        IMessageService messageService, IEmailService emailService, Guid? activationId)
        : base(scrollService)
    {
        _messageService = messageService;
        _emailService = emailService;

        _activationId = activationId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(MessageAdded payload) => await Replace(payload.Id, payload.ActivationId);

    public async Task Handle(MessageSaved payload) => await Replace(payload.Id, payload.ActivationId);

    public async Task Handle(MessageRemoved payload) => await Rid(payload.Id, payload.ActivationId);
    public async Task Handle(EmailAdded payload) => await Refresh();
    public async Task Handle(EmailSaved payload) => await Refresh();
    public async Task Handle(EmailRemoved payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Activation>(new() { Id = _activationId });
        var response = await WithWaiting("Fetching...", () => _messageService.FetchForActivation(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Where(x => x.EmailId.HasValue).Select(x => new MessageModel(x)).ToList());
    }

    public override async Task<Response<MessageModel?>> Fetch(Guid? id)
    {
        var response = await _messageService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new MessageModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        var emailId = Cards.FirstOrDefault(x => x.Entity.Id == id)?.Entity.Message.EmailId;
        return emailId.HasValue
            ? await _emailService.Remove(new(new() { Id = emailId }))
            : new("Email not found.");
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_activationId);
    }
}

public class MessageModel : Observable, IDisposable, INamed
{
    private void HandleMessageChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Message));

    private Message _message;

    public String? Name => Message.Name ?? Message.StageName;

    public String? PopulationName { get; }

    public MessageModel(Message message, String? populationName = null)
    {
        _message = message;
        PopulationName = populationName;
        _message.PropertyChanged += HandleMessageChanged;
    }

    public void Dispose()
    {
        _message.PropertyChanged -= HandleMessageChanged;
    }

    public Guid? Id
    {
        get => _message.Id;
        set => _message.Id = value;
    }

    public Message Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
}