using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class MembershipListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IMembershipService MembershipService { get; set; } = null!;

    public MembershipListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, MembershipService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class MembershipListForPortalModel : ListModel<MembershipModel>,
    IHandle<MembershipAdded>, IHandle<MembershipSaved>, IHandle<MembershipRemoved>,
    IHandle<MemberAdded>, IHandle<MemberRemoved>,
    IHandle<EmailAdded>, IHandle<EmailRemoved>,
    IHandle<EmailTemplateAdded>, IHandle<EmailTemplateRemoved>
{
    private readonly IMembershipService _membershipService;
    private readonly Guid? _portalId;

    public MembershipListForPortalModel(IEventBus eventBus, IScrollService scrollService, IMembershipService membershipService, Guid? portalId)
        : base(scrollService)
    {
        _membershipService = membershipService;
        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(MembershipAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(MembershipSaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(MembershipRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public async Task Handle(MemberAdded payload)
    {
        if (payload.MembershipId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.MembershipId)))
            await Replace(payload.MembershipId);
    }

    public async Task Handle(MemberRemoved payload)
    {
        if (payload.MembershipId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.MembershipId)))
            await Replace(payload.MembershipId);
    }

    public async Task Handle(EmailAdded payload)
    {
        if (payload.MembershipId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.MembershipId)))
            await Replace(payload.MembershipId);
    }

    public async Task Handle(EmailRemoved payload)
    {
        if (payload.MembershipId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.MembershipId)))
            await Replace(payload.MembershipId);
    }

    public async Task Handle(EmailTemplateAdded payload)
    {
        if (payload.MembershipId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.MembershipId)))
            await Replace(payload.MembershipId);
    }

    public async Task Handle(EmailTemplateRemoved payload)
    {
        if (payload.MembershipId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.MembershipId)))
            await Replace(payload.MembershipId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _membershipService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new MembershipModel(x)).ToList());
    }

    public override async Task<Response<MembershipModel?>> Fetch(Guid? id)
    {
        var response = await _membershipService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new MembershipModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _membershipService.Remove(new(new()
        {
            Id = id,
            PortalId = _portalId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_portalId);
    }
}

public class MembershipModel : Observable, IDisposable, INamed
{
    private void HandleMembershipChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Membership));

    private Membership _membership;

    public String? Name => Membership.Name;

    public MembershipModel(Membership membership)
    {
        _membership = membership;
        _membership.PropertyChanged += HandleMembershipChanged;
    }

    public void Dispose()
    {
        _membership.PropertyChanged -= HandleMembershipChanged;
    }

    public Guid? Id
    {
        get => _membership.Id;
        set => _membership.Id = value;
    }

    public Membership Membership
    {
        get => _membership;
        set => SetProperty(ref _membership, value);
    }
}