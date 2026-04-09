namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class ContentPortalEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IContentPortalService ContentPortalService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;

    public ContentPortalEditModel Model { get; set; } = null!;

    public IList<PortalFeature> Features => (Model.Entity?.Portal.Features ?? [])
        .Where(x => x.PermissionId is null || SessionState.Session.Permissions.Contains(x.PermissionId.GetValueOrDefault()))
        .Where(x => !x.Key.IsBasically("details"))
        .OrderBy(x => x.Title)
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ContentPortalService);
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

public class ContentPortalEditModel : EditModel<ContentPortal>, IHandle<ContentPortalSaved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IContentPortalService _contentPortalService;

    public ContentPortalEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IContentPortalService contentPortalService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _contentPortalService = contentPortalService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ContentPortalSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public OverrideState MaxWidthState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _contentPortalService.Fetch(new(new() { Id = _id })));

        if (response.Ok)
            SetContentPortal(response.Value);
    }

    public async Task Save()
    {
        if (MaxWidthState == OverrideState.Default)
            Entity!.MaxWidth = null;

        var response = await WithWaiting("Saving...", () => _contentPortalService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private void SetContentPortal(ContentPortal contentPortal)
    {
        Entity = contentPortal;

        MaxWidthState = Entity.MaxWidth.HasSomething() ? OverrideState.Custom : OverrideState.Default;

        _navigator.UpdateTitle(_path, Entity.Portal.Title!);
    }
}