namespace Crudspa.Content.Design.Client.Plugins.PaneType;

using Publication = Shared.Contracts.Data.Publication;

public partial class PublicationListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPublicationService PublicationService { get; set; } = null!;

    public PublicationListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PublicationService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PublicationListForPortalModel : ListOrderablesModel<PublicationModel>,
    IHandle<PublicationAdded>, IHandle<PublicationSaved>, IHandle<PublicationRemoved>, IHandle<PublicationsReordered>
{
    private readonly IPublicationService _publicationService;
    private readonly Guid? _portalId;

    public PublicationListForPortalModel(IEventBus eventBus, IScrollService scrollService, IPublicationService publicationService, Guid? portalId)
        : base(scrollService)
    {
        _publicationService = publicationService;

        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PublicationAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(PublicationSaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(PublicationRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public async Task Handle(PublicationsReordered payload) => await Refresh();





    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _publicationService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new PublicationModel(x)).ToList());
    }

    public override async Task<Response<PublicationModel?>> Fetch(Guid? id)
    {
        var response = await _publicationService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new PublicationModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _publicationService.Remove(new(new()
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
        var orderables = Cards.Select(x => x.Entity.Publication).ToList();
        return await WithWaiting("Saving...", () => _publicationService.SaveOrder(new(orderables)));
    }
}

public class PublicationModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandlePublicationChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Publication));

    private Publication _publication;

    public String? Name => Publication.Title;

    public PublicationModel(Publication publication)
    {
        _publication = publication;
        _publication.PropertyChanged += HandlePublicationChanged;
    }

    public void Dispose()
    {
        _publication.PropertyChanged -= HandlePublicationChanged;
    }

    public Guid? Id
    {
        get => _publication.Id;
        set => _publication.Id = value;
    }

    public Int32? Ordinal
    {
        get => _publication.Ordinal;
        set => _publication.Ordinal = value;
    }

    public Publication Publication
    {
        get => _publication;
        set => SetProperty(ref _publication, value);
    }
}