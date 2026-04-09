namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using Publisher = Shared.Contracts.Data.Publisher;

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
        Model = new(EventBus, PublisherService);
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

public class PublisherEditModel : EditModel<Publisher>,
    IHandle<PublisherSaved>
{
    private readonly IPublisherService _publisherService;

    public List<Named> PermissionNames = [];

    public PublisherEditModel(IEventBus eventBus,
        IPublisherService publisherService) : base(false)
    {
        _publisherService = publisherService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PublisherSaved payload)
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

        var response = await WithWaiting("Fetching...", () => _publisherService.Fetch(new()));

        if (response.Ok)
            SetPublisher(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _publisherService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _publisherService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetPublisher(Publisher publisher)
    {
        Entity = publisher;
    }
}