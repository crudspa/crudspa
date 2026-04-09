namespace Crudspa.Education.School.Client.Plugins.PaneType;

public partial class StudentFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IStudentService StudentService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public StudentFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, StudentService);
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
        Navigator.GoTo($"{Path}/student-{Guid.NewGuid():D}?state=new");
    }
}

public class StudentFindModel : FindModel<StudentSearch, Student>,
    IHandle<StudentAdded>, IHandle<StudentSaved>, IHandle<StudentRemoved>
{
    private readonly IStudentService _studentService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public StudentFindModel(IEventBus eventBus, IScrollService scrollService, IStudentService studentService)
        : base(scrollService)
    {
        _studentService = studentService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "First Name",
            "Last Name",
            "ID Number",
        ];
    }

    public async Task Handle(StudentAdded payload) => await Refresh();
    public async Task Handle(StudentSaved payload) => await Refresh();
    public async Task Handle(StudentRemoved payload) => await Refresh();

    public List<Orderable> GradeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> AssessmentLevelNames
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

        Search.Grades.Clear();
        Search.AssessmentLevelGroups.Clear();

        await WithMany("Initializing...",
            FetchGradeNames(),
            FetchAssessmentLevelNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<StudentSearch>(Search);
        var response = await WithWaiting("Searching...", () => _studentService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _studentService.Remove(new(new() { Id = id })));
    }

    public async Task FetchGradeNames()
    {
        var response = await WithAlerts(() => _studentService.FetchGrades(new()), false);
        if (response.Ok) GradeNames = response.Value.ToList();
    }

    public async Task FetchAssessmentLevelNames()
    {
        var response = await WithAlerts(() => _studentService.FetchAssessmentLevels(new()), false);
        if (response.Ok) AssessmentLevelNames = response.Value.ToList();
    }
}