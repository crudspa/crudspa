using Crudspa.Framework.Jobs.Shared.Contracts.Ids;

namespace Crudspa.Framework.Jobs.Client.Plugins.PaneType;

public partial class JobEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IJobService JobService { get; set; } = null!;
    [Inject] public IJobCopyService JobCopyService { get; set; } = null!;

    public JobEditModel Model { get; set; } = null!;
    public JobDesignPlugin DesignComponent { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, JobService, JobCopyService);
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
        await Model.Save((IJobDesign)DesignComponent.Instance!);
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class JobEditModel : EditModel<Job>, IHandle<JobSaved>, IHandle<JobRemoved>, IHandle<JobStatusChanged>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IJobService _jobService;
    private readonly IJobCopyService _jobCopyService;
    private Boolean _watchingProgress;

    public JobEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IJobService jobService,
        IJobCopyService jobCopyService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _jobService = jobService;
        _jobCopyService = jobCopyService;

        eventBus.Subscribe(this);

        Progress = new() { Message = "..." };
    }

    public async Task Handle(JobSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public async Task Handle(JobStatusChanged payload)
    {
        if (!payload.Id.Equals(_id))
            return;

        if (Entity is null)
        {
            await Refresh();
            return;
        }

        Entity.ApplyStatusChange(payload, JobStatusNames, DeviceNames);
        SetProgressForStatus();
        RaisePropertyChanged(nameof(Entity));
        RaisePropertyChanged(nameof(Progress));
    }

    public Task Handle(JobRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public Progress Progress { get; }

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

    public List<OrderableCssClass> JobStatusNames
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
            FetchDeviceNames(),
            FetchJobStatusNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var copy = _jobCopyService.Pop();

            if (copy is not null)
                SetJob(copy);
            else
                SetJob(new()
                {
                    Id = Guid.NewGuid(),
                    TypeId = JobTypes.MinBy(x => x.Name)?.Id,
                    TypeName = "New Job",
                    StatusId = JobStatusIds.Pending,
                    Description = String.Empty,
                });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _jobService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetJob(response.Value);
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
            var response = await WithWaiting("Adding...", () => _jobService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/job-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
    }

    public void CopyJob()
    {
        _jobCopyService.Push(Entity!);
        _navigator.GoTo($"{_path?.Parent()}/job-{Guid.NewGuid():D}?state=new");
    }

    public async Task RepeatJob()
    {
        var job = Entity!.DeepClone();

        job.Id = Guid.NewGuid();

        var response = await WithWaiting("Adding...", () => _jobService.Add(new(job)));

        if (response.Ok)
        {
            _navigator.GoTo($"{_path.Parent()}/job-{response.Value.Id:D}");
            _navigator.Close(_path);
        }
    }

    public async Task FetchJobTypes()
    {
        var response = await WithAlerts(() => _jobService.FetchJobTypes(new()), false);
        if (response.Ok) JobTypes = response.Value.ToList();
    }

    public async Task FetchDeviceNames()
    {
        var response = await WithAlerts(() => _jobService.FetchDeviceNames(new()), false);
        if (response.Ok) DeviceNames = response.Value.ToList();
    }

    public async Task FetchJobStatusNames()
    {
        var response = await WithAlerts(() => _jobService.FetchJobStatusNames(new()), false);
        if (response.Ok) JobStatusNames = response.Value.ToList();
    }

    private void SetSelectedType()
    {
        if (Entity is not null && JobTypes.HasItems())
            SelectedJobType = JobTypes.FirstOrDefault(x => x.Id.Equals(Entity.TypeId));
    }

    public void SetJobType(Guid? jobTypeId)
    {
        Entity!.TypeId = jobTypeId;
        SetSelectedType();
    }

    private void SetJob(Job job)
    {
        Entity = job;

        SetSelectedType();
        SetProgressForStatus();

        var title = IsNew ? "New Job" : Entity.TypeName + " | " + Entity.Description;
        _navigator.UpdateTitle(_path, title);
    }

    private void SetProgressForStatus()
    {
        if (Entity is null)
            return;

        Progress.EntityId = Entity.Id;

        switch (Entity.StatusId)
        {
            case var id when id == JobStatusIds.Pending:
                _watchingProgress = true;
                Progress.Message = "Pending";
                Progress.Percentage = 0;
                break;
            case var id when id == JobStatusIds.Running:
                _watchingProgress = true;
                if (Progress.Message == "...")
                    Progress.Message = "Running...";
                if (Progress.Percentage == 0)
                    Progress.Percentage = 5;
                break;
            case var id when id == JobStatusIds.Completed:
                if (!_watchingProgress)
                    Progress.Message = "Completed";
                Progress.Percentage = 100;
                break;
            case var id when id == JobStatusIds.Failed:
                if (!_watchingProgress)
                    Progress.Message = "Failed";
                Progress.Percentage = 100;
                break;
            case var id when id == JobStatusIds.Canceled:
                if (!_watchingProgress)
                    Progress.Message = "Canceled";
                Progress.Percentage = 100;
                break;
            default:
                throw new($"JobStatusId has not been added to {nameof(JobEditModel)}. ({Entity.StatusId:D})");
        }
    }
}