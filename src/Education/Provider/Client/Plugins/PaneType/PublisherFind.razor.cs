namespace Crudspa.Education.Provider.Client.Plugins.PaneType;

public partial class PublisherFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPublisherService PublisherService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public PublisherFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PublisherService);
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
        Navigator.GoTo($"{Path}/publisher-{Guid.NewGuid():D}?state=new");
    }
}

public class PublisherFindModel : FindModel<PublisherSearch, Publisher>,
    IHandle<PublisherAdded>, IHandle<PublisherSaved>, IHandle<PublisherRemoved>,
    IHandle<PublisherContactAdded>, IHandle<PublisherContactRemoved>
{
    private readonly IPublisherService _publisherService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public PublisherFindModel(IEventBus eventBus, IScrollService scrollService, IPublisherService publisherService)
        : base(scrollService)
    {
        _publisherService = publisherService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Name",
        ];
    }

    public async Task Handle(PublisherAdded payload) => await Refresh();

    public async Task Handle(PublisherSaved payload) => await Refresh();

    public async Task Handle(PublisherRemoved payload) => await Refresh();

    public async Task Handle(PublisherContactAdded payload) => await Refresh();

    public async Task Handle(PublisherContactRemoved payload) => await Refresh();

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

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

        var request = new Request<PublisherSearch>(Search);
        var response = await WithWaiting("Searching...", () => _publisherService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _publisherService.Remove(new(new() { Id = id })));
    }
}