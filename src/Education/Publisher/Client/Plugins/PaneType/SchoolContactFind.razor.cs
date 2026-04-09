namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class SchoolContactFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISchoolContactService SchoolContactService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public SchoolContactFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SchoolContactService);
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
        Navigator.GoTo($"{Path}/school-contact-{Guid.NewGuid():D}?state=new");
    }
}

public class SchoolContactFindModel : FindModel<SchoolContactSearch, SchoolContact>,
    IHandle<SchoolContactAdded>, IHandle<SchoolContactSaved>, IHandle<SchoolContactRemoved>
{
    private readonly ISchoolContactService _schoolContactService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public SchoolContactFindModel(IEventBus eventBus, IScrollService scrollService, ISchoolContactService schoolContactService)
        : base(scrollService)
    {
        _schoolContactService = schoolContactService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "First",
            "Last",
            "Username",
        ];
    }

    public async Task Handle(SchoolContactAdded payload) => await Refresh();

    public async Task Handle(SchoolContactSaved payload) => await Refresh();

    public async Task Handle(SchoolContactRemoved payload) => await Refresh();

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

        var request = new Request<SchoolContactSearch>(Search);
        var response = await WithWaiting("Searching...", () => _schoolContactService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _schoolContactService.Remove(new(new() { Id = id })));
    }
}