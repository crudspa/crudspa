namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class SchoolFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISchoolService SchoolService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public SchoolFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SchoolService);
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
        Navigator.GoTo($"{Path}/school-{Guid.NewGuid():D}?state=new");
    }
}

public class SchoolFindModel : FindModel<SchoolSearch, School>,
    IHandle<SchoolAdded>, IHandle<SchoolSaved>, IHandle<SchoolRemoved>,
    IHandle<CommunityAdded>, IHandle<CommunitySaved>, IHandle<CommunityRemoved>,
    IHandle<ClassroomAdded>, IHandle<ClassroomRemoved>,
    IHandle<SchoolContactAdded>, IHandle<SchoolContactRemoved>
{
    private readonly ISchoolService _schoolService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public SchoolFindModel(IEventBus eventBus, IScrollService scrollService, ISchoolService schoolService)
        : base(scrollService)
    {
        _schoolService = schoolService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Name",
            "Key",
        ];
    }

    public async Task Handle(SchoolAdded payload) => await Refresh();

    public async Task Handle(SchoolSaved payload) => await Refresh();

    public async Task Handle(SchoolRemoved payload) => await Refresh();

    public async Task Handle(CommunityAdded payload) => await FetchCommunityNames();

    public async Task Handle(CommunitySaved payload) => await FetchCommunityNames();

    public async Task Handle(CommunityRemoved payload) => await FetchCommunityNames();

    public async Task Handle(ClassroomAdded payload) => await Refresh();

    public async Task Handle(ClassroomRemoved payload) => await Refresh();

    public async Task Handle(SchoolContactAdded payload) => await Refresh();

    public async Task Handle(SchoolContactRemoved payload) => await Refresh();

    public ObservableCollection<Named> CommunityNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

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
        Search.Communities.Clear();

        await WithMany("Initializing...",
            FetchCommunityNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<SchoolSearch>(Search);
        var response = await WithWaiting("Searching...", () => _schoolService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _schoolService.Remove(new(new() { Id = id })));
    }

    public async Task FetchCommunityNames()
    {
        var response = await WithAlerts(() => _schoolService.FetchCommunityNames(new()), false);
        if (response.Ok) CommunityNames = response.Value.ToObservable();
    }
}