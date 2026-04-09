namespace Crudspa.Education.Provider.Client.Plugins.PaneType;

public partial class PublisherEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPublisherService PublisherService { get; set; } = null!;

    public PublisherEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, PublisherService);
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

public class PublisherEditModel : EditModel<Publisher>,
    IHandle<PublisherSaved>, IHandle<PublisherRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IPublisherService _publisherService;

    public List<Named> PermissionNames = [];

    public PublisherEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IPublisherService publisherService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _publisherService = publisherService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PublisherSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(PublisherRemoved payload)
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

            var publisher = new Publisher
            {
                Organization = new()
                {
                    Name = "New Organization",
                    TimeZoneId = "America/New_York",
                },
            };

            SetPublisher(publisher);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _publisherService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetPublisher(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _publisherService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/publisher-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _publisherService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _publisherService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetPublisher(Publisher publisher)
    {
        Entity = publisher;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}