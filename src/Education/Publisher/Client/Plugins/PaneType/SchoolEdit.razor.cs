namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class SchoolEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISchoolService SchoolService { get; set; } = null!;

    public SchoolEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var districtId = Path!.Id("district");

        Model = new(Path, Id, IsNew, districtId, EventBus, Navigator, SchoolService);
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

public class SchoolEditModel : EditModel<School>,
    IHandle<SchoolSaved>, IHandle<SchoolRemoved>,
    IHandle<CommunityAdded>, IHandle<CommunitySaved>, IHandle<CommunityRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _districtId;
    private readonly INavigator _navigator;
    private readonly ISchoolService _schoolService;

    public List<Named> PermissionNames = [];

    public SchoolEditModel(String? path, Guid? id, Boolean isNew, Guid? districtId,
        IEventBus eventBus,
        INavigator navigator,
        ISchoolService schoolService) : base(isNew)
    {
        _path = path;
        _id = id;
        _districtId = districtId;
        _navigator = navigator;
        _schoolService = schoolService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SchoolSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(SchoolRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(CommunityAdded payload) => await FetchCommunityNames();

    public async Task Handle(CommunitySaved payload) => await FetchCommunityNames();

    public async Task Handle(CommunityRemoved payload) => await FetchCommunityNames();

    public ObservableCollection<Named> CommunityNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchPermissionNames(),
            FetchCommunityNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var school = new School
            {
                DistrictId = _districtId,
                Key = String.Empty,
                Treatment = false,
                Organization = new()
                {
                    Name = "New Organization",
                    TimeZoneId = "America/New_York",
                },
            };

            SetSchool(school);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _schoolService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetSchool(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _schoolService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/school-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _schoolService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchCommunityNames()
    {
        var response = await WithAlerts(() => _schoolService.FetchCommunityNames(new()), false);
        if (response.Ok) CommunityNames = response.Value.ToObservable();
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _schoolService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetSchool(School school)
    {
        Entity = school;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}