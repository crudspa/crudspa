namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class CommunityList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ICommunityService CommunityService { get; set; } = null!;

    public CommunityListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, CommunityService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CommunityListModel : ListModel<CommunityModel>,
    IHandle<CommunityAdded>, IHandle<CommunitySaved>, IHandle<CommunityRemoved>,
    IHandle<CommunityRelationsSaved>
{
    private readonly ICommunityService _communityService;

    public CommunityListModel(IEventBus eventBus, IScrollService scrollService, ICommunityService communityService)
        : base(scrollService)
    {
        _communityService = communityService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CommunityAdded payload) => await Replace(payload.Id);

    public async Task Handle(CommunitySaved payload) => await Replace(payload.Id);

    public async Task Handle(CommunityRemoved payload) => await Rid(payload.Id);

    public async Task Handle(CommunityRelationsSaved payload) => await Refresh(false);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request(Guid.Empty);
        var response = await WithWaiting("Fetching...", () => _communityService.FetchAll(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new CommunityModel(x)).ToList());
    }

    public override async Task<Response<CommunityModel?>> Fetch(Guid? id)
    {
        var response = await _communityService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new CommunityModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _communityService.Remove(new(new() { Id = id }));
    }
}

public class CommunityModel : Observable, IDisposable, INamed
{
    private void HandleCommunityChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Community));

    private Community _community;

    public String? Name => Community.Name;

    public CommunityModel(Community community)
    {
        _community = community;
        _community.PropertyChanged += HandleCommunityChanged;
    }

    public void Dispose()
    {
        _community.PropertyChanged -= HandleCommunityChanged;
    }

    public Guid? Id
    {
        get => _community.Id;
        set => _community.Id = value;
    }

    public Community Community
    {
        get => _community;
        set => SetProperty(ref _community, value);
    }
}