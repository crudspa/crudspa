namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SmsMessageThread : IPaneDisplay, IDisposable
{
    private Boolean _scrollToBottom;

    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is nameof(SmsMessageThreadModel.Messages)
            or nameof(SmsMessageThreadModel.Media)
            or nameof(SmsMessageThreadModel.PendingReplyBody)
            or nameof(SmsMessageThreadModel.SendingReply))
            _scrollToBottom = true;

        _ = InvokeAsync(StateHasChanged);
    }

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISmsMessageService SmsMessageService { get; set; } = null!;
    [Inject] public ISmsMessageMediaService SmsMessageMediaService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public SmsMessageThreadModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SmsMessageService, SmsMessageMediaService, Navigator, Path, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
        _scrollToBottom = true;
    }

    protected override async Task OnAfterRenderAsync(Boolean firstRender)
    {
        if (!_scrollToBottom)
            return;

        _scrollToBottom = false;

        await ScrollService.ToElementBottom(Model.MessagesId);
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SmsMessageThreadModel : ScreenModel,
    IHandle<SmsMessageAdded>, IHandle<SmsMessageSaved>, IHandle<SmsMessageRemoved>
{
    private readonly ISmsMessageService _smsMessageService;
    private readonly ISmsMessageMediaService _smsMessageMediaService;
    private readonly INavigator _navigator;
    private readonly String? _path;
    private readonly Guid? _smsMessageId;

    public SmsMessageThreadModel(IEventBus eventBus, IScrollService scrollService, ISmsMessageService smsMessageService,
        ISmsMessageMediaService smsMessageMediaService,
        INavigator navigator, String? path, Guid? smsMessageId)
    {
        _smsMessageService = smsMessageService;
        _smsMessageMediaService = smsMessageMediaService;
        _navigator = navigator;
        _path = path;
        _smsMessageId = smsMessageId;
        eventBus.Subscribe(this);
    }

    public ObservableCollection<SmsMessage> Messages
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String BottomId { get; } = $"sms-thread-bottom-{Guid.NewGuid():N}";
    public String MessagesId { get; } = $"sms-thread-messages-{Guid.NewGuid():N}";

    public String? ReplyBody
    {
        get;
        set
        {
            SetProperty(ref field, value);
            RaisePropertyChanged(nameof(ReplyDisabled));
        }
    }

    public String? PendingReplyBody
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean SendingReply
    {
        get;
        set
        {
            SetProperty(ref field, value);
            RaisePropertyChanged(nameof(ReplyDisabled));
        }
    }

    public Boolean ReplyDisabled => SendingReply || ReplyBody.HasNothing();

    public Dictionary<Guid, IList<SmsMessageMedia>> Media
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public IList<SmsMessageMedia> MediaFor(Guid? smsMessageId)
    {
        return smsMessageId.HasValue && Media.TryGetValue(smsMessageId.Value, out var media)
            ? media
            : [];
    }

    public String DirectionClass(SmsMessage message) =>
        message.Direction == SmsMessage.Directions.Inbound ? "inbound" : "outbound";

    public String StatusClass(SmsMessage.Statuses status) =>
        status switch
        {
            SmsMessage.Statuses.Queued or SmsMessage.Statuses.Sending => "pending",
            SmsMessage.Statuses.Undelivered or SmsMessage.Statuses.Failed => "failed",
            SmsMessage.Statuses.Canceled => "canceled",
            _ => "complete",
        };

    public Boolean ShowStatus(SmsMessage message) =>
        message.Direction == SmsMessage.Directions.Outbound || StatusClass(message.Status) != "complete";

    public String OccurredLabel(SmsMessage message) =>
        message.Occurred?.ToString("g") ?? "Pending";

    public async Task Handle(SmsMessageAdded payload) => await Refresh();

    public async Task Handle(SmsMessageSaved payload) => await Refresh();

    public async Task Handle(SmsMessageRemoved payload) => await Refresh();

    public async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<SmsMessage>(new() { Id = _smsMessageId });
        var response = await WithWaiting("Loading...", () => _smsMessageService.SearchThread(request), resetAlerts);

        if (response.Ok)
        {
            Messages = response.Value.OrderBy(x => x.Occurred).ToObservable();
            await FetchMedia();
            _navigator.UpdateTitle(_path, Messages.FirstOrDefault()?.ConversationName ?? "Text Thread");
        }
    }

    private async Task FetchMedia()
    {
        var media = new Dictionary<Guid, IList<SmsMessageMedia>>();

        foreach (var message in Messages.Where(x => x.Id.HasValue))
        {
            var response = await WithAlerts(() => _smsMessageMediaService.FetchForSmsMessage(new(new() { Id = message.Id })), false);
            if (response.Ok && response.Value.HasItems())
                media[message.Id!.Value] = response.Value;
        }

        Media = media;
    }

    public async Task SendReply()
    {
        if (ReplyDisabled)
            return;

        var replyBody = ReplyBody;
        ReplyBody = String.Empty;
        PendingReplyBody = replyBody;
        SendingReply = true;

        var response = await WithAlerts(() => _smsMessageService.Reply(new(new()
        {
            Id = _smsMessageId,
            Body = replyBody,
        })));

        if (response.Ok || response.Value is not null)
        {
            await Refresh(false);
        }
        else
        {
            ReplyBody = replyBody;
        }

        PendingReplyBody = null;
        SendingReply = false;
    }
}