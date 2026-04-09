namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class CommunityRelations : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ICommunityService CommunityService { get; set; } = null!;

    public CommunityRelationsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, EventBus, CommunityService);
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
        await Model.Refresh();
    }
}

public class CommunityRelationsModel : EditModel<Community>, IHandle<CommunityRelationsSaved>
{
    private readonly Guid? _id;
    private readonly ICommunityService _communityService;

    public CommunityRelationsModel(Guid? id,
        IEventBus eventBus,
        ICommunityService communityService) : base(false)
    {
        _id = id;
        _communityService = communityService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CommunityRelationsSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _communityService.FetchSchoolSelections(new(new() { Id = _id })));

        if (response.Ok)
            SetCommunity(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _communityService.SaveSchoolSelections(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private void SetCommunity(Community community)
    {
        Entity = community;
    }
}