namespace Crudspa.Education.School.Client.Plugins.PaneType;

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

    public AssessmentAssignmentFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SessionState, AssessmentAssignmentService);
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
    private void HandleBulkModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BulkModel));
    private void HandleResetProgressModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ResetProgressModel));

    private readonly IAssessmentAssignmentService _assessmentAssignmentService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public AssessmentAssignmentFindModel(IEventBus eventBus, IScrollService scrollService, ISessionState sessionState, IAssessmentAssignmentService assessmentAssignmentService)
        : base(scrollService)
    {
        _assessmentAssignmentService = assessmentAssignmentService;

        eventBus.Subscribe(this);

        _sorts =
        [
            "First Name",
            "Last Name",
        ];

        BulkModel = new(scrollService, sessionState);
        BulkModel.PropertyChanged += HandleBulkModelChanged;

        ResetProgressModel = new(scrollService);
        ResetProgressModel.PropertyChanged += HandleResetProgressModelChanged;
    }

    public override void Dispose()
    {
        BulkModel.PropertyChanged -= HandleBulkModelChanged;
        ResetProgressModel.PropertyChanged -= HandleResetProgressModelChanged;
        base.Dispose();
    }

    public BulkModalModel BulkModel { get; set; }
    public ResetProgressModalModel ResetProgressModel { get; }

    public async Task Handle(AssessmentAssignmentAdded payload) => await Refresh();
    public async Task Handle(AssessmentAssignmentSaved payload) => await Refresh();
    public async Task Handle(AssessmentAssignmentRemoved payload) => await Refresh();

    public List<Named> ClassroomNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

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
            FetchClassroomNames(),
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

    public Task ShowResetProgressConfirmation(Guid? id)
    {
        ResetProgressModel.AssessmentAssignmentId = id;
        ResetProgressModel.Visible = true;
        return Task.CompletedTask;
    }

    public async Task ResetProgress()
    {
        if (!ResetProgressModel.AssessmentAssignmentId.HasValue)
            return;

        var response = await WithWaiting("Resetting progress...", () =>
            _assessmentAssignmentService.ResetProgress(new(new() { Id = ResetProgressModel.AssessmentAssignmentId })));

        if (response.Ok)
            await ResetProgressModel.Hide();
    }

    public async Task FetchClassroomNames()
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchClassroomNames(new()), false);
        if (response.Ok)
        {
            ClassroomNames = response.Value.ToList();
            if (ClassroomNames.HasItems())
                BulkModel.DefaultClassroomId = ClassroomNames.MinBy(x => x.Name)!.Id;
        }
    }

    public async Task FetchAssessmentNames()
    {
        var response = await WithAlerts(() => _assessmentAssignmentService.FetchAssessmentNames(new()), false);
        if (response.Ok)
        {
            AssessmentNames = response.Value.ToList();
            if (AssessmentNames.HasItems())
                BulkModel.DefaultAssessmentId = AssessmentNames.MinBy(x => x.Name)!.Id;
        }
    }

    public async Task BulkAssign()
    {
        var response = await WithWaiting(() => _assessmentAssignmentService.BulkAssign(new(BulkModel.Assignment)));
        if (response.Ok)
        {
            BulkModel.Assignment = response.Value;
            BulkModel.Completed = true;
            await Refresh();
        }
    }
}

public class BulkModalModel : ModalModel
{
    private readonly ISessionState _sessionState;
    private AssessmentAssignmentBulk _assignment = new();

    private void HandleAssignmentChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Assignment));

    public BulkModalModel(IScrollService scrollService, ISessionState sessionState) : base(scrollService)
    {
        _sessionState = sessionState;
        _assignment.PropertyChanged += HandleAssignmentChanged;
    }

    public override void Dispose()
    {
        _assignment.PropertyChanged -= HandleAssignmentChanged;
        base.Dispose();
    }

    public Guid? DefaultClassroomId { get; set; }
    public Guid? DefaultAssessmentId { get; set; }

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

    public override Task Show()
    {
        Completed = false;

        Assignment.Scope = AssessmentAssignmentBulk.Scopes.Classroom;
        Assignment.ClassroomId = DefaultClassroomId;
        Assignment.AssessmentId = DefaultAssessmentId;
        Assignment.Action = AssessmentAssignmentBulk.Actions.Add;
        Assignment.SetDefaultDates(_sessionState.TimeZoneId);

        return base.Show();
    }
}

public class ResetProgressModalModel(IScrollService scrollService) : ModalModel(scrollService)
{
    public Guid? AssessmentAssignmentId
    {
        get;
        set => SetProperty(ref field, value);
    }
}