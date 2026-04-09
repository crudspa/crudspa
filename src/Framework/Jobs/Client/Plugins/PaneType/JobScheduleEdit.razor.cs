namespace Crudspa.Framework.Jobs.Client.Plugins.PaneType;

public partial class JobScheduleEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IJobScheduleService JobScheduleService { get; set; } = null!;

    public JobScheduleEditModel Model { get; set; } = null!;
    public JobDesignPlugin EditorComponent { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, JobScheduleService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleSaveClicked()
    {
        await Model.Save((IJobDesign)EditorComponent.Instance!);
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class JobScheduleEditModel : EditModel<JobSchedule>, IHandle<JobScheduleSaved>, IHandle<JobScheduleRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IJobScheduleService _jobScheduleService;

    public JobScheduleEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IJobScheduleService jobScheduleService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _jobScheduleService = jobScheduleService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(JobScheduleSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(JobScheduleRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<JobTypeFull> JobTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Named> DeviceNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public JobTypeFull? SelectedJobType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchJobTypes(),
            FetchDeviceNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetJobSchedule(new()
            {
                Name = "New Schedule",
                DeviceId = null,
                TypeId = JobTypes.MinBy(x => x.Name)?.Id,
                Config = null,
                Description = null,
                RecurrenceAmount = 1,
                RecurrenceInterval = JobSchedule.RecurrenceIntervals.Day,
                RecurrencePattern = JobSchedule.RecurrencePatterns.Simple,
                Day = 1,
                Hour = 22,
                Minute = 0,
                Second = 0,
                DayOfWeek = JobSchedule.DayOfWeeks.Sunday,
                TimeZoneId = "America/Denver",
                NextRun = DateTimeOffset.Now,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _jobScheduleService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetJobSchedule(response.Value);
        }
    }

    public async Task Save(IJobDesign design)
    {
        if (!IsValid(design))
            return;

        Entity!.Config = design.GetConfigJson();
        Entity!.Description = design.Description;

        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _jobScheduleService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/job-schedule-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _jobScheduleService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchJobTypes()
    {
        var response = await WithAlerts(() => _jobScheduleService.FetchJobTypes(new()), false);
        if (response.Ok) JobTypes = response.Value.ToList();
    }

    public async Task FetchDeviceNames()
    {
        var response = await WithAlerts(() => _jobScheduleService.FetchDeviceNames(new()), false);
        if (response.Ok) DeviceNames = response.Value.ToList();
    }

    public void SetJobType(Guid? jobTypeId)
    {
        Entity!.TypeId = jobTypeId;
        SetSelectedType();
    }

    private void SetSelectedType()
    {
        if (Entity is not null && JobTypes.HasItems())
            SelectedJobType = JobTypes.FirstOrDefault(x => x.Id.Equals(Entity.TypeId));
    }

    private void SetJobSchedule(JobSchedule jobSchedule)
    {
        Entity = jobSchedule;

        SetSelectedType();

        _navigator.UpdateTitle(_path, Entity.Name!);
    }
}