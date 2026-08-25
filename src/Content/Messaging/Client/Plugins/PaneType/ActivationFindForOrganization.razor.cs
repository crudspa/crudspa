namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class ActivationFindForOrganization : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IActivationService ActivationService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public ActivationFindForOrganizationModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var organizationId = Id ?? SessionState.Session.User?.OrganizationId;
        Model = new(EventBus, ScrollService, ActivationService, organizationId);
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
        Navigator.GoTo($"{Path}/activation-{Guid.NewGuid():D}?state=new");
    }
}

public class ActivationFindForOrganizationModel : FindModel<ActivationSearch, Activation>,
    IHandle<ActivationAdded>, IHandle<ActivationSaved>, IHandle<ActivationRemoved>
{
    private readonly IActivationService _activationService;
    private readonly Guid? _organizationId;
    private Boolean _resetting;

    public ActivationFindForOrganizationModel(IEventBus eventBus, IScrollService scrollService, IActivationService activationService, Guid? organizationId)
        : base(scrollService)
    {
        _activationService = activationService;

        _organizationId = organizationId;
        eventBus.Subscribe(this);
    }

    public async Task Handle(ActivationAdded payload) => await Refresh();

    public async Task Handle(ActivationSaved payload) => await Refresh();

    public async Task Handle(ActivationRemoved payload) => await Refresh();

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _organizationId;

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

        var request = new Request<ActivationSearch>(Search);
        var response = await WithWaiting("Searching...", () => _activationService.SearchForOrganization(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _activationService.Remove(new(new() { Id = id })));
    }
}