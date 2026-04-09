namespace Crudspa.Education.Provider.Client.Plugins.PaneType;

public partial class PublisherContactEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPublisherContactService PublisherContactService { get; set; } = null!;

    public PublisherContactEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var publisherId = Path!.Id("publisher");

        Model = new(Path, Id, IsNew, publisherId, EventBus, Navigator, PublisherContactService);
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

public class PublisherContactEditModel : EditModel<PublisherContact>,
    IHandle<PublisherContactSaved>, IHandle<PublisherContactRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _publisherId;
    private readonly INavigator _navigator;
    private readonly IPublisherContactService _publisherContactService;

    public List<Named> RoleNames = [];

    public PublisherContactEditModel(String? path, Guid? id, Boolean isNew, Guid? publisherId,
        IEventBus eventBus,
        INavigator navigator,
        IPublisherContactService publisherContactService) : base(isNew)
    {
        _path = path;
        _id = id;
        _publisherId = publisherId;
        _navigator = navigator;
        _publisherContactService = publisherContactService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PublisherContactSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(PublisherContactRemoved payload)
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

            var publisherContact = new PublisherContact
            {
                PublisherId = _publisherId,
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
                publisherContact.User.Roles.Add(new()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Selected = false,
                });

            SetPublisherContact(publisherContact);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _publisherContactService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
            {
                var publisherContact = response.Value;

                if (publisherContact.User.Roles.HasNothing())
                    foreach (var role in RoleNames)
                        publisherContact.User.Roles.Add(new()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            Selected = false,
                        });

                SetPublisherContact(publisherContact);
            }
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _publisherContactService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/publisher-contact-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _publisherContactService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchRoleNames()
    {
        var response = await WithAlerts(() => _publisherContactService.FetchRoleNames(new(new() { Id = _publisherId })), false);
        if (response.Ok) RoleNames = response.Value.ToList();
    }

    public async Task SendAccessCode()
    {
        var response = await WithWaiting("Sending...", () => _publisherContactService.SendAccessCode(new(Entity!)));

        if (response.Ok)
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Success,
                Message = "Access code sent.",
            });
    }

    private void SetPublisherContact(PublisherContact publisherContact)
    {
        Entity = publisherContact;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}