using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class LicenseEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ILicenseService LicenseService { get; set; } = null!;

    public LicenseEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, LicenseService);
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

public class LicenseEditModel : EditModel<License>,
    IHandle<LicenseSaved>, IHandle<LicenseRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ILicenseService _licenseService;

    public LicenseEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        ILicenseService licenseService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _licenseService = licenseService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(LicenseSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(LicenseRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var license = new License
            {
                Name = "New License",
            };

            SetLicense(license);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _licenseService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetLicense(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _licenseService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/license-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _licenseService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void SetLicense(License license)
    {
        Entity = license;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}