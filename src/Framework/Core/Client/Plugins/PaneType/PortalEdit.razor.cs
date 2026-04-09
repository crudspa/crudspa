namespace Crudspa.Framework.Core.Client.Plugins.PaneType;

public partial class PortalEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPortalService PortalService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;

    public PortalEditModel Model { get; set; } = null!;

    public IList<PortalFeature> Features => (Model.Entity?.Features ?? [])
        .Where(x => x.PermissionId is null || SessionState.Session.Permissions.Contains(x.PermissionId.GetValueOrDefault()))
        .Where(x => !x.Key.IsBasically("details"))
        .OrderBy(x => x.Title)
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, PortalService);
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

public class PortalEditModel : EditModel<Portal>, IHandle<PortalSaved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IPortalService _portalService;

    public PortalEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IPortalService portalService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _portalService = portalService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PortalSaved payload)
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

        var response = await WithWaiting("Fetching...", () => _portalService.Fetch(new(new() { Id = _id })));

        if (response.Ok)
            SetPortal(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _portalService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private void SetPortal(Portal portal)
    {
        Entity = portal;

        _navigator.UpdateTitle(_path, Entity.Title!);
    }
}