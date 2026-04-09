namespace Crudspa.Framework.Jobs.Client.Plugins.PaneType;

public partial class JobFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IJobService JobService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public JobFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, JobService);
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
        Navigator.GoTo($"{Path}/job-{Guid.NewGuid():D}?state=new");
    }
}

public class JobFindModel : FindModel<JobSearch, Job>,
    IHandle<JobAdded>, IHandle<JobSaved>, IHandle<JobRemoved>
{
    private readonly IJobService _jobService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public JobFindModel(IEventBus eventBus, IScrollService scrollService, IJobService jobService)
        : base(scrollService)
    {
        _jobService = jobService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Added",
            "Started",
        ];
    }

    public async Task Handle(JobAdded payload) => await Refresh();
    public async Task Handle(JobSaved payload) => await Refresh();
    public async Task Handle(JobRemoved payload) => await Refresh();

    public List<Named> JobTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<OrderableCssClass> JobStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Named> DeviceNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> JobScheduleNames
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
        Search.Sort.Ascending = false;

        Search.Types.Clear();
        Search.Status.Clear();
        Search.Devices.Clear();
        Search.Schedules.Clear();
        Search.AddedRange.Type = DateRange.Types.InTheLastDay;

        await WithMany("Initializing...",
            FetchJobTypeNames(),
            FetchJobStatusNames(),
            FetchDeviceNames(),
            FetchJobScheduleNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<JobSearch>(Search);
        var response = await WithWaiting("Searching...", () => _jobService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _jobService.Remove(new(new() { Id = id })));
    }

    public async Task FetchJobTypeNames()
    {
        var response = await WithAlerts(() => _jobService.FetchJobTypeNames(new()), false);
        if (response.Ok) JobTypeNames = response.Value.ToList();
    }

    public async Task FetchJobStatusNames()
    {
        var response = await WithAlerts(() => _jobService.FetchJobStatusNames(new()), false);
        if (response.Ok) JobStatusNames = response.Value.ToList();
    }

    public async Task FetchDeviceNames()
    {
        var response = await WithAlerts(() => _jobService.FetchDeviceNames(new()), false);
        if (response.Ok) DeviceNames = response.Value.ToList();
    }

    public async Task FetchJobScheduleNames()
    {
        var response = await WithAlerts(() => _jobService.FetchJobScheduleNames(new()), false);
        if (response.Ok) JobScheduleNames = response.Value.ToObservable();
    }
}