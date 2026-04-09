namespace Crudspa.Education.Provider.Client.Plugins.PaneType;

public partial class PublisherContactFindForPublisher : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPublisherContactService PublisherContactService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public PublisherContactFindForPublisherModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PublisherContactService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void AddNew()
    {
        Navigator.GoTo($"{Path}/publisher-contact-{Guid.NewGuid():D}?state=new");
    }
}

public class PublisherContactFindForPublisherModel : FindModel<PublisherContactSearch, PublisherContact>,
    IHandle<PublisherContactAdded>, IHandle<PublisherContactSaved>, IHandle<PublisherContactRemoved>
{
    private readonly IPublisherContactService _publisherContactService;
    private readonly Guid? _publisherId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public PublisherContactFindForPublisherModel(IEventBus eventBus, IScrollService scrollService, IPublisherContactService publisherContactService, Guid? publisherId)
        : base(scrollService)
    {
        _publisherContactService = publisherContactService;

        _publisherId = publisherId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "First",
            "Last",
            "Username",
        ];
    }

    public async Task Handle(PublisherContactAdded payload) => await Refresh();

    public async Task Handle(PublisherContactSaved payload) => await Refresh();

    public async Task Handle(PublisherContactRemoved payload) => await Refresh();

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _publisherId;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = true;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<PublisherContactSearch>(Search);
        var response = await WithWaiting("Searching...", () => _publisherContactService.SearchForPublisher(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _publisherContactService.Remove(new(new() { Id = id })));
    }
}