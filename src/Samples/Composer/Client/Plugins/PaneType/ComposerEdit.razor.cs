namespace Crudspa.Samples.Composer.Client.Plugins.PaneType;

using Composer = Shared.Contracts.Data.Composer;

public partial class ComposerEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IComposerService ComposerService { get; set; } = null!;

    public ComposerEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ComposerService);
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

public class ComposerEditModel : EditModel<Composer>,
    IHandle<ComposerSaved>
{
    private readonly IComposerService _composerService;

    public List<Named> PermissionNames = [];

    public ComposerEditModel(IEventBus eventBus,
        IComposerService composerService) : base(false)
    {
        _composerService = composerService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ComposerSaved payload)
    {
        if (payload.Id.Equals(Entity?.Id))
            await Refresh();
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchPermissionNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _composerService.Fetch(new()));

        if (response.Ok)
            SetComposer(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _composerService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _composerService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetComposer(Composer composer)
    {
        Entity = composer;
    }
}