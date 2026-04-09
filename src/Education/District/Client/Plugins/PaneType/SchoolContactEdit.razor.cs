namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class SchoolContactEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISchoolContactService SchoolContactService { get; set; } = null!;

    public SchoolContactEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var schoolId = Path!.Id("school");

        Model = new(Path, Id, IsNew, schoolId, EventBus, Navigator, SchoolContactService);
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

public class SchoolContactEditModel : EditModel<SchoolContact>,
    IHandle<SchoolContactSaved>, IHandle<SchoolContactRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _schoolId;
    private readonly INavigator _navigator;
    private readonly ISchoolContactService _schoolContactService;

    public List<Named> RoleNames = [];

    public SchoolContactEditModel(String? path, Guid? id, Boolean isNew, Guid? schoolId,
        IEventBus eventBus,
        INavigator navigator,
        ISchoolContactService schoolContactService) : base(isNew)
    {
        _path = path;
        _id = id;
        _schoolId = schoolId;
        _navigator = navigator;
        _schoolContactService = schoolContactService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SchoolContactSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(SchoolContactRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<Orderable> TitleNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchRoleNames(),
            FetchTitleNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var schoolContact = new SchoolContact
            {
                SchoolId = _schoolId,
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
                schoolContact.User.Roles.Add(new()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Selected = false,
                });

            SetSchoolContact(schoolContact);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _schoolContactService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
            {
                var schoolContact = response.Value;

                if (schoolContact.User.Roles.HasNothing())
                    foreach (var role in RoleNames)
                        schoolContact.User.Roles.Add(new()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            Selected = false,
                        });

                SetSchoolContact(schoolContact);
            }
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _schoolContactService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/school-contact-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _schoolContactService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchTitleNames()
    {
        var response = await WithAlerts(() => _schoolContactService.FetchTitleNames(new()), false);
        if (response.Ok) TitleNames = response.Value.ToList();
    }

    public async Task FetchRoleNames()
    {
        var response = await WithAlerts(() => _schoolContactService.FetchRoleNames(new(new() { Id = _schoolId })), false);
        if (response.Ok) RoleNames = response.Value.ToList();
    }

    public async Task SendAccessCode()
    {
        var response = await WithWaiting("Sending...", () => _schoolContactService.SendAccessCode(new(Entity!)));

        if (response.Ok)
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Success,
                Message = "Access code sent.",
            });
    }

    private void SetSchoolContact(SchoolContact schoolContact)
    {
        Entity = schoolContact;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}