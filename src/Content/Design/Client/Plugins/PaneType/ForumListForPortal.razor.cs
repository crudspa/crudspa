namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class ForumListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IForumService ForumService { get; set; } = null!;

    public ForumListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ForumService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ForumListForPortalModel : ListOrderablesModel<ForumModel>,
    IHandle<ForumAdded>, IHandle<ForumSaved>, IHandle<ForumRemoved>
{
    private readonly IForumService _forumService;
    private readonly Guid? _portalId;

    public ForumListForPortalModel(IEventBus eventBus, IScrollService scrollService, IForumService forumService, Guid? portalId)
        : base(scrollService)
    {
        _forumService = forumService;
        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ForumAdded payload) => await Replace(payload.Id, payload.PortalId);
    public async Task Handle(ForumSaved payload) => await Replace(payload.Id, payload.PortalId);
    public async Task Handle(ForumRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _forumService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ForumModel(x)).ToList());
    }

    public override async Task<Response<ForumModel?>> Fetch(Guid? id)
    {
        var response = await _forumService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ForumModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _forumService.Remove(new(new()
        {
            Id = id,
            PortalId = _portalId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_portalId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Forum).ToList();
        return await WithWaiting("Saving...", () => _forumService.SaveOrder(new(orderables)));
    }
}

public class ForumModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleForumChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Forum));

    private Forum _forum;

    public String? Name => Forum.Title;

    public ForumModel(Forum forum)
    {
        _forum = forum;
        _forum.PropertyChanged += HandleForumChanged;
    }

    public void Dispose()
    {
        _forum.PropertyChanged -= HandleForumChanged;
    }

    public Guid? Id
    {
        get => _forum.Id;
        set => _forum.Id = value;
    }

    public Int32? Ordinal
    {
        get => _forum.Ordinal;
        set => _forum.Ordinal = value;
    }

    public Forum Forum
    {
        get => _forum;
        set => SetProperty(ref _forum, value);
    }
}