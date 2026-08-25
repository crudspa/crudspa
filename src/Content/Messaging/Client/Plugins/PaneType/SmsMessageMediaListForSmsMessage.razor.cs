namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class SmsMessageMediaListForSmsMessage : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISmsMessageMediaService SmsMessageMediaService { get; set; } = null!;

    public SmsMessageMediaListForSmsMessageModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SmsMessageMediaService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SmsMessageMediaListForSmsMessageModel : ListOrderablesModel<SmsMessageMediaModel>,
    IHandle<SmsMessageMediasReordered>
{
    private readonly ISmsMessageMediaService _smsMessageMediaService;
    private readonly Guid? _smsMessageId;

    public SmsMessageMediaListForSmsMessageModel(IEventBus eventBus, IScrollService scrollService, ISmsMessageMediaService smsMessageMediaService, Guid? smsMessageId)
        : base(scrollService)
    {
        _smsMessageMediaService = smsMessageMediaService;

        _smsMessageId = smsMessageId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SmsMessageMediasReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<SmsMessage>(new() { Id = _smsMessageId });
        var response = await WithWaiting("Fetching...", () => _smsMessageMediaService.FetchForSmsMessage(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new SmsMessageMediaModel(x)).ToList());
    }

    public override async Task<Response<SmsMessageMediaModel?>> Fetch(Guid? id)
    {
        return await Task.FromResult(new Response<SmsMessageMediaModel?>("Sms message media records are read-only."));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await Task.FromResult(new Response("Sms message media records are read-only."));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_smsMessageId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.SmsMessageMedia).ToList();
        return await WithWaiting("Saving...", () => _smsMessageMediaService.SaveOrder(new(orderables)));
    }
}

public class SmsMessageMediaModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleSmsMessageMediaChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(SmsMessageMedia));

    private SmsMessageMedia _smsMessageMedia;

    public String? Name => SmsMessageMedia.FileName;

    public SmsMessageMediaModel(SmsMessageMedia smsMessageMedia)
    {
        _smsMessageMedia = smsMessageMedia;
        _smsMessageMedia.PropertyChanged += HandleSmsMessageMediaChanged;
    }

    public void Dispose()
    {
        _smsMessageMedia.PropertyChanged -= HandleSmsMessageMediaChanged;
    }

    public Guid? Id
    {
        get => _smsMessageMedia.Id;
        set => _smsMessageMedia.Id = value;
    }

    public Int32? Ordinal
    {
        get => _smsMessageMedia.Ordinal;
        set => _smsMessageMedia.Ordinal = value;
    }

    public SmsMessageMedia SmsMessageMedia
    {
        get => _smsMessageMedia;
        set => SetProperty(ref _smsMessageMedia, value);
    }
}