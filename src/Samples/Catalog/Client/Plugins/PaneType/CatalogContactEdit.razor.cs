namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class CatalogContactEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ICatalogContactService CatalogContactService { get; set; } = null!;

    public CatalogContactEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, CatalogContactService);
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

public class CatalogContactEditModel : EditModel<CatalogContact>,
    IHandle<CatalogContactSaved>, IHandle<CatalogContactRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ICatalogContactService _catalogContactService;

    public List<Named> RoleNames = [];

    public CatalogContactEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        ICatalogContactService catalogContactService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _catalogContactService = catalogContactService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(CatalogContactSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(CatalogContactRemoved payload)
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

            var catalogContact = new CatalogContact
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
                catalogContact.User.Roles.Add(new()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Selected = false,
                });

            SetCatalogContact(catalogContact);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _catalogContactService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
            {
                var catalogContact = response.Value;

                if (catalogContact.User.Roles.HasNothing())
                    foreach (var role in RoleNames)
                        catalogContact.User.Roles.Add(new()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            Selected = false,
                        });

                SetCatalogContact(catalogContact);
            }
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _catalogContactService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/catalog-contact-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _catalogContactService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchRoleNames()
    {
        var response = await WithAlerts(() => _catalogContactService.FetchRoleNames(new()), false);
        if (response.Ok) RoleNames = response.Value.ToList();
    }

    public async Task SendAccessCode()
    {
        var response = await WithWaiting("Sending...", () => _catalogContactService.SendAccessCode(new(Entity!)));

        if (response.Ok)
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Success,
                Message = "Access code sent.",
            });
    }

    private void SetCatalogContact(CatalogContact catalogContact)
    {
        Entity = catalogContact;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}