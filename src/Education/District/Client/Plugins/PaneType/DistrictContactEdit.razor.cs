namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class DistrictContactEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IDistrictContactService DistrictContactService { get; set; } = null!;

    public DistrictContactEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, DistrictContactService);
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

public class DistrictContactEditModel : EditModel<DistrictContact>,
    IHandle<DistrictContactSaved>, IHandle<DistrictContactRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IDistrictContactService _districtContactService;

    public List<Named> RoleNames = [];

    public DistrictContactEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IDistrictContactService districtContactService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _districtContactService = districtContactService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(DistrictContactSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(DistrictContactRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchRoleNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var districtContact = new DistrictContact
            {
                Contact = new()
                {
                    FirstName = "New",
                    LastName = "Contact",
                    TimeZoneId = "America/New_York",
                },
                User = new()
                {
                    MaySignIn = true,
                    Username = String.Empty,
                    ResetPassword = true,
                },
            };

            foreach (var role in RoleNames)
                districtContact.User.Roles.Add(new()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Selected = false,
                });

            SetDistrictContact(districtContact);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _districtContactService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
            {
                var districtContact = response.Value;

                if (districtContact.User.Roles.HasNothing())
                    foreach (var role in RoleNames)
                        districtContact.User.Roles.Add(new()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            Selected = false,
                        });

                SetDistrictContact(districtContact);
            }
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _districtContactService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/district-contact-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _districtContactService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchRoleNames()
    {
        var response = await WithAlerts(() => _districtContactService.FetchRoleNames(new()), false);
        if (response.Ok) RoleNames = response.Value.ToList();
    }

    public async Task SendAccessCode()
    {
        var response = await WithWaiting("Sending...", () => _districtContactService.SendAccessCode(new(Entity!)));

        if (response.Ok)
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Success,
                Message = "Access code sent.",
            });
    }

    private void SetDistrictContact(DistrictContact districtContact)
    {
        Entity = districtContact;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}