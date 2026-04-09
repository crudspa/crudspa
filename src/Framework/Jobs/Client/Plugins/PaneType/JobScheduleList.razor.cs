using Crudspa.Framework.Jobs.Shared.Extensions;

namespace Crudspa.Framework.Jobs.Client.Plugins.PaneType;

public partial class JobScheduleList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IJobScheduleService JobScheduleService { get; set; } = null!;

    public JobScheduleListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, JobScheduleService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class JobScheduleListModel : ListModel<JobScheduleModel>,
    IHandle<JobScheduleAdded>, IHandle<JobScheduleSaved>, IHandle<JobScheduleRemoved>
{
    private readonly IJobScheduleService _jobScheduleService;

    public JobScheduleListModel(IEventBus eventBus, IScrollService scrollService, IJobScheduleService jobScheduleService)
        : base(scrollService)
    {
        _jobScheduleService = jobScheduleService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(JobScheduleAdded payload) => await Replace(payload.Id);
    public async Task Handle(JobScheduleSaved payload) => await Replace(payload.Id);
    public async Task Handle(JobScheduleRemoved payload) => await Rid(payload.Id);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request(Guid.Empty);
        var response = await WithWaiting("Fetching...", () => _jobScheduleService.FetchAll(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new JobScheduleModel(x)).ToList());
    }

    public override async Task<Response<JobScheduleModel?>> Fetch(Guid? id)
    {
        var response = await _jobScheduleService.Fetch(new(new() { Id = id }));

        if (!response.Ok)
            return new() { Errors = response.Errors };

        return new(new JobScheduleModel(response.Value));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _jobScheduleService.Remove(new(new() { Id = id }));
    }
}

public class JobScheduleModel : Observable, IDisposable, INamed
{
    private JobSchedule _jobSchedule;

    public String? Name => JobSchedule.Name;

    public JobScheduleModel(JobSchedule jobSchedule)
    {
        _jobSchedule = jobSchedule;
        _jobSchedule.PropertyChanged += HandleJobScheduleChanged;
    }

    public void Dispose()
    {
        _jobSchedule.PropertyChanged -= HandleJobScheduleChanged;
    }

    private void HandleJobScheduleChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(JobSchedule));
    }

    public Guid? Id
    {
        get => _jobSchedule.Id;
        set => _jobSchedule.Id = value;
    }

    public JobSchedule JobSchedule
    {
        get => _jobSchedule;
        set => SetProperty(ref _jobSchedule, value);
    }

    public String JobDescription => JobSchedule.TypeName + " | " + JobSchedule.Description;

    public String ScheduleDescription => JobSchedule.BuildDescription();
}