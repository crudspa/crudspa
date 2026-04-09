namespace Crudspa.Samples.Composer.Client.Plugins.PaneType;

public partial class ComposerContactEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IComposerContactService ComposerContactService { get; set; } = null!;

    public ComposerContactEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ComposerContactService);
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

public class ComposerContactEditModel : EditModel<ComposerContact>,
    IHandle<ComposerContactSaved>, IHandle<ComposerContactRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IComposerContactService _composerContactService;

    public List<Named> RoleNames = [];

    public ComposerContactEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IComposerContactService composerContactService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _composerContactService = composerContactService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ComposerContactSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ComposerContactRemoved payload)
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

            var composerContact = new ComposerContact
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
                composerContact.User.Roles.Add(new()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Selected = false,
                });

            SetComposerContact(composerContact);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _composerContactService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
            {
                var composerContact = response.Value;

                if (composerContact.User.Roles.HasNothing())
                    foreach (var role in RoleNames)
                        composerContact.User.Roles.Add(new()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            Selected = false,
                        });

                SetComposerContact(composerContact);
            }
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _composerContactService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/composer-contact-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _composerContactService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchRoleNames()
    {
        var response = await WithAlerts(() => _composerContactService.FetchRoleNames(new()), false);
        if (response.Ok) RoleNames = response.Value.ToList();
    }

    public async Task SendAccessCode()
    {
        var response = await WithWaiting("Sending...", () => _composerContactService.SendAccessCode(new(Entity!)));

        if (response.Ok)
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Success,
                Message = "Access code sent.",
            });
    }

    private void SetComposerContact(ComposerContact composerContact)
    {
        Entity = composerContact;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}