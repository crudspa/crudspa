namespace Crudspa.Education.Provider.Client.Plugins.PaneType;

using Provider = Shared.Contracts.Data.Provider;

public partial class ProviderEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IProviderService ProviderService { get; set; } = null!;

    public ProviderEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ProviderService);
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

public class ProviderEditModel : EditModel<Provider>,
    IHandle<ProviderSaved>
{
    private readonly IProviderService _providerService;

    public List<Named> PermissionNames = [];

    public ProviderEditModel(IEventBus eventBus,
        IProviderService providerService) : base(false)
    {
        _providerService = providerService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ProviderSaved payload)
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

        var response = await WithWaiting("Fetching...", () => _providerService.Fetch(new()));

        if (response.Ok)
            SetProvider(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _providerService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _providerService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetProvider(Provider provider)
    {
        Entity = provider;
    }
}