namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class CommunityListForDistrict : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ICommunityService CommunityService { get; set; } = null!;

    public CommunityListForDistrictModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, CommunityService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CommunityListForDistrictModel : ListModel<CommunityModel>,
    IHandle<CommunityAdded>, IHandle<CommunitySaved>, IHandle<CommunityRemoved>,
    IHandle<CommunityRelationsSaved>
{
    private readonly ICommunityService _communityService;
    private readonly Guid? _districtId;

    public CommunityListForDistrictModel(IEventBus eventBus, IScrollService scrollService, ICommunityService communityService, Guid? districtId)
        : base(scrollService)
    {
        _communityService = communityService;
        _districtId = districtId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CommunityAdded payload) => await Replace(payload.Id, payload.DistrictId);

    public async Task Handle(CommunitySaved payload) => await Replace(payload.Id, payload.DistrictId);

    public async Task Handle(CommunityRemoved payload) => await Rid(payload.Id, payload.DistrictId);

    public async Task Handle(CommunityRelationsSaved payload) => await Refresh(false);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<District>(new() { Id = _districtId });
        var response = await WithWaiting("Fetching...", () => _communityService.FetchForDistrict(request), resetAlerts);

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
        return await _communityService.Remove(new(new()
        {
            Id = id,
            DistrictId = _districtId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_districtId);
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