namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

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
        Model = new(Path, Id, IsNew, EventBus, Navigator, DistrictService);
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
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class DistrictEditModel : EditModel<District>,
    IHandle<DistrictSaved>, IHandle<DistrictRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IDistrictService _districtService;

    public List<Named> PermissionNames = [];

    public DistrictEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IDistrictService districtService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _districtService = districtService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(DistrictSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(DistrictRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchPermissionNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var district = new District
            {
                StudentIdNumberLabel = String.Empty,
                AssessmentExplainer = String.Empty,
                Organization = new()
                {
                    Name = "New Organization",
                    TimeZoneId = "America/New_York",
                },
            };

            SetDistrict(district);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _districtService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetDistrict(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _districtService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/district-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _districtService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _districtService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetDistrict(District district)
    {
        Entity = district;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}