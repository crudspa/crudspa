
namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PublicationGroupListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPublicationGroupService PublicationGroupService { get; set; } = null!;

    public PublicationGroupListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PublicationGroupService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PublicationGroupListForPortalModel : ListOrderablesModel<PublicationGroupModel>,
    IHandle<PublicationGroupAdded>, IHandle<PublicationGroupSaved>, IHandle<PublicationGroupRemoved>, IHandle<PublicationGroupsReordered>
{
    private readonly IPublicationGroupService _publicationGroupService;
    private readonly Guid? _portalId;

    public PublicationGroupListForPortalModel(IEventBus eventBus, IScrollService scrollService, IPublicationGroupService publicationGroupService, Guid? portalId)
        : base(scrollService)
    {
        _publicationGroupService = publicationGroupService;

        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PublicationGroupAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(PublicationGroupSaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(PublicationGroupRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public async Task Handle(PublicationGroupsReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _publicationGroupService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new PublicationGroupModel(x)).ToList());
    }

    public override async Task<Response<PublicationGroupModel?>> Fetch(Guid? id)
    {
        var response = await _publicationGroupService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new PublicationGroupModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _publicationGroupService.Remove(new(new()
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
        var orderables = Cards.Select(x => x.Entity.PublicationGroup).ToList();
        return await WithWaiting("Saving...", () => _publicationGroupService.SaveOrder(new(orderables)));
    }
}

public class PublicationGroupModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandlePublicationGroupChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(PublicationGroup));

    private PublicationGroup _publicationGroup;

    public String? Name => PublicationGroup.Name;

    public PublicationGroupModel(PublicationGroup publicationGroup)
    {
        _publicationGroup = publicationGroup;
        _publicationGroup.PropertyChanged += HandlePublicationGroupChanged;
    }

    public void Dispose()
    {
        _publicationGroup.PropertyChanged -= HandlePublicationGroupChanged;
    }

    public Guid? Id
    {
        get => _publicationGroup.Id;
        set => _publicationGroup.Id = value;
    }

    public Int32? Ordinal
    {
        get => _publicationGroup.Ordinal;
        set => _publicationGroup.Ordinal = value;
    }

    public PublicationGroup PublicationGroup
    {
        get => _publicationGroup;
        set => SetProperty(ref _publicationGroup, value);
    }
}