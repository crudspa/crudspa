namespace Crudspa.Education.District.Client.Plugins.PaneType;

using District = Shared.Contracts.Data.District;

public partial class DistrictEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IDistrictService DistrictService { get; set; } = null!;

    public DistrictEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, DistrictService);
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

public class DistrictEditModel : EditModel<District>,
    IHandle<DistrictSaved>
{
    private readonly IDistrictService _districtService;

    public List<Named> PermissionNames = [];

    public DistrictEditModel(IEventBus eventBus,
        IDistrictService districtService) : base(false)
    {
        _districtService = districtService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(DistrictSaved payload)
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

        var response = await WithWaiting("Fetching...", () => _districtService.Fetch(new()));

        if (response.Ok)
            SetDistrict(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _districtService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _districtService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetDistrict(District district)
    {
        Entity = district;
    }
}