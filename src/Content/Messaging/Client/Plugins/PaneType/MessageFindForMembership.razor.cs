namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class MessageFindForMembership : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IMessageService MessageService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public MessageFindForMembershipModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, MessageService, Id);
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
        Navigator.GoTo($"{Path}/message-{Guid.NewGuid():D}?state=new");
    }
}

public class MessageFindForMembershipModel : FindModel<MessageSearch, Message>,
    IHandle<MessageAdded>, IHandle<MessageSaved>, IHandle<MessageRemoved>
{
    private readonly IMessageService _messageService;
    private readonly Guid? _membershipId;
    private Boolean _resetting;

    public MessageFindForMembershipModel(IEventBus eventBus, IScrollService scrollService, IMessageService messageService, Guid? membershipId)
        : base(scrollService)
    {
        _messageService = messageService;

        _membershipId = membershipId;
        eventBus.Subscribe(this);
    }

    public async Task Handle(MessageAdded payload) => await Refresh();

    public async Task Handle(MessageSaved payload) => await Refresh();

    public async Task Handle(MessageRemoved payload) => await Refresh();

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

        var request = new Request<MessageSearch>(Search);
        var response = await WithWaiting("Searching...", () => _messageService.SearchForMembership(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _messageService.Remove(new(new() { Id = id })));
    }
}