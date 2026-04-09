namespace Crudspa.Framework.Core.Client.Plugins.PaneType;

public partial class PortalList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPortalService PortalService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;

    public PortalListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PortalService, SessionState.Session.Permissions);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PortalListModel : ListModel<PortalModel>,
    IHandle<PortalSaved>
{
    private readonly IPortalService _portalService;
    private readonly IEnumerable<Guid> _permissions;

    public PortalListModel(IEventBus eventBus, IScrollService scrollService, IPortalService portalService, IEnumerable<Guid> permissions)
        : base(scrollService)
    {
        _portalService = portalService;
        _permissions = permissions;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PortalSaved payload) => await Replace(payload.Id);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request(Guid.Empty);
        var response = await WithWaiting("Fetching...", () => _portalService.FetchAll(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new PortalModel(x, _permissions)).ToList());
    }

    public override async Task<Response<PortalModel?>> Fetch(Guid? id)
    {
        var response = await _portalService.Fetch(new(new() { Id = id }));

        if (!response.Ok)
            return new() { Errors = response.Errors };

        return new(new PortalModel(response.Value, _permissions));
    }

    public override Task<Response> Remove(Guid? id)
    {
        return Task.FromResult(new Response());
    }
}

public class PortalModel : Observable, IDisposable, INamed
{
    private Portal _portal;

    public String? Name => $"{Portal.Key ?? String.Empty} | {Portal.Title ?? String.Empty}";

    public PortalModel(Portal portal, IEnumerable<Guid> permissions)
    {
        _portal = portal;
        _portal.PropertyChanged += HandlePortalChanged;

        Features = portal.Features
            .Where(x => x.PermissionId is null || permissions.Contains(x.PermissionId.GetValueOrDefault()))
            .Where(x => !x.Key.IsBasically("details"))
            .OrderBy(x => x.Title)
            .ToObservable();
    }

    public void Dispose()
    {
        _portal.PropertyChanged -= HandlePortalChanged;
    }

    private void HandlePortalChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(Portal));
    }

    public Guid? Id
    {
        get => _portal.Id;
        set => _portal.Id = value;
    }

    public Portal Portal
    {
        get => _portal;
        set => SetProperty(ref _portal, value);
    }

    public ObservableCollection<PortalFeature> Features
    {
        get;
        set => SetProperty(ref field, value);
    }
}