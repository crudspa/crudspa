namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class CampaignEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ICampaignService CampaignService { get; set; } = null!;

    public CampaignEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal");

        Model = new(Path, Id, IsNew, portalId, EventBus, Navigator, CampaignService);
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

public class CampaignEditModel : EditModel<Campaign>,
    IHandle<CampaignSaved>, IHandle<CampaignRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly ICampaignService _campaignService;

    public List<Named> LicenseNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public CampaignEditModel(String? path, Guid? id, Boolean isNew, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        ICampaignService campaignService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _navigator = navigator;
        _campaignService = campaignService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CampaignSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(CampaignRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await FetchLicenseNames();
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var campaign = new Campaign
            {
                PortalId = _portalId,
                Name = "New Campaign",
            };

            foreach (var license in LicenseNames)
            {
                campaign.Licenses.Add(new()
                {
                    Id = license.Id,
                    Name = license.Name,
                    Selected = false,
                });
            }

            SetCampaign(campaign);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _campaignService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetCampaign(response.Value);
        }
    }

    private async Task FetchLicenseNames()
    {
        var response = await WithAlerts(() => _campaignService.FetchLicenseNames(new()), false);
        if (response.Ok)
            LicenseNames = response.Value.ToList();
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _campaignService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/campaign-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _campaignService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public Task StartActivation()
    {
        _navigator.GoTo($"{_path}/activate");
        return Task.CompletedTask;
    }


    private void SetCampaign(Campaign campaign)
    {
        Entity = campaign;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}