namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class AssessmentAssignmentFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IAssessmentAssignmentService AssessmentAssignmentService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ILogger<AssessmentAssignmentFind> Logger { get; set; } = null!;

    public AssessmentAssignmentFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SessionState, AssessmentAssignmentService, Logger);
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
        Navigator.GoTo($"{Path}/assessment-assignment-{Guid.NewGuid():D}?state=new");
    }
}

public class AssessmentAssignmentFindModel : FindModel<AssessmentAssignmentSearch, AssessmentAssignment>,
    IHandle<AssessmentAssignmentAdded>, IHandle<AssessmentAssignmentSaved>, IHandle<AssessmentAssignmentRemoved>
{
    private async void HandleBulkModelChanged(Object? sender, PropertyChangedEventArgs args)
    {
        try
        {
            RaisePropertyChanged(nameof(BulkModel));

            if (args.PropertyName == nameof(BulkModel.Completed) && BulkModel.Completed)
                await Refresh();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in event handler.");
        }
    }

    private readonly IAssessmentAssignmentService _assessmentAssignmentService;
    private readonly ILogger _logger;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public AssessmentAssignmentFindModel(IEventBus eventBus, IScrollService scrollService, ISessionState sessionState, IAssessmentAssignmentService assessmentAssignmentService, ILogger logger)
        : base(scrollService)
    {
        _assessmentAssignmentService = assessmentAssignmentService;
        _logger = logger;

        eventBus.Subscribe(this);

        _sorts =
        [
            "First Name",
            "Last Name",
        ];

        BulkModel = new(scrollService, sessionState, assessmentAssignmentService, _logger);
        BulkModel.PropertyChanged += HandleBulkModelChanged;
    }

    public override void Dispose()
    {
        BulkModel.PropertyChanged -= HandleBulkModelChanged;
        base.Dispose();
    }

    public BulkModalModel BulkModel { get; set; }

    public async Task Handle(AssessmentAssignmentAdded payload) => await Refresh();
    public async Task Handle(AssessmentAssignmentSaved payload) => await Refresh();
    public async Task Handle(AssessmentAssignmentRemoved payload) => await Refresh();

    public List<Named> AssessmentNames
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

        Search.Classrooms.Clear();
        Search.Assessments.Clear();

        await WithMany("Initializing...",
            FetchAssessmentNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<AssessmentAssignmentSearch>(Search);
        var response = await WithWaiting("Searching...", () => _assessmentAssignmentService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _assessmentAssignmentService.Remove(new(new() { Id = id })));
    }

    public async Task FetchAssessmentNames()
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchAssessmentNames(new()), false);
        if (response.Ok) AssessmentNames = response.Value.ToList();
    }
}

public class BulkModalModel : ModalModel
{
    private readonly ISessionState _sessionState;
    private readonly IAssessmentAssignmentService _assessmentAssignmentService;
    private readonly ILogger _logger;
    private AssessmentAssignmentBulk _assignment = new();

    private void HandleAssignmentChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Assignment));

    public BulkModalModel(IScrollService scrollService, ISessionState sessionState, IAssessmentAssignmentService assessmentAssignmentService, ILogger logger) : base(scrollService)
    {
        _sessionState = sessionState;
        _assessmentAssignmentService = assessmentAssignmentService;
        _logger = logger;
        _assignment.PropertyChanged += HandleAssignmentChanged;
    }

    public override void Dispose()
    {
        _assignment.PropertyChanged -= HandleAssignmentChanged;
        base.Dispose();
    }

    public Boolean Completed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AssessmentAssignmentBulk Assignment
    {
        get => _assignment;
        set => SetProperty(ref _assignment, value);
    }

    public async void HandleSchoolChanged(Guid? id)
    {
        try
        {
            _assignment.SchoolId = id;
            await FetchClassroomNames(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in event handler.");
        }
    }

    public override async Task Show()
    {
        Assignment.Scope = AssessmentAssignmentBulk.Scopes.Classroom;
        Assignment.AssessmentId = null;
        Assignment.ClassroomId = null;
        Assignment.SchoolId = null;
        Assignment.ClassroomId = null;
        Assignment.Action = AssessmentAssignmentBulk.Actions.Add;
        Assignment.SetDefaultDates(_sessionState.TimeZoneId);
        Assignment.RecordsAdded = 0;
        Assignment.RecordsUpdated = 0;

        Completed = false;

        await base.Show();

        await Refresh();
    }

    private async Task Refresh()
    {
        await WithMany("Initializing...",
            FetchAssessmentNames(),
            FetchCommunityNames(),
            FetchSchoolNames());
    }

    public List<Named> AssessmentNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Named> ClassroomNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Named> CommunityNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Named> SchoolNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task FetchAssessmentNames()
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchAssessmentNames(new()), false);
        if (response.Ok) AssessmentNames = response.Value.ToList();
    }

    public async Task FetchClassroomNames(Guid? schoolId)
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchClassroomNames(new(new() { Id = schoolId })), false);
        if (response.Ok) ClassroomNames = response.Value.ToList();
    }

    public async Task FetchCommunityNames()
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchCommunityNames(new()), false);
        if (response.Ok) CommunityNames = response.Value.ToList();
    }

    public async Task FetchSchoolNames()
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchSchoolNames(new()), false);
        if (response.Ok) SchoolNames = response.Value.ToList();
    }

    public async Task BulkAssign()
    {
        var response = await WithWaiting(() => _assessmentAssignmentService.BulkAssign(new(Assignment)));
        if (response.Ok)
        {
            Assignment = response.Value;
            Completed = true;
        }
    }
}