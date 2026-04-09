namespace Crudspa.Education.School.Client.Plugins.PaneType;

public partial class StudentEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IStudentService StudentService { get; set; } = null!;

    public StudentEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, StudentService);
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

public class StudentEditModel : EditModel<Student>, IHandle<StudentSaved>, IHandle<StudentRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IStudentService _studentService;

    public StudentEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IStudentService studentService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _studentService = studentService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(StudentSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(StudentRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

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

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchGradeNames(),
            FetchAssessmentLevelNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetStudent(new()
            {
                FirstName = "New",
                LastName = "Student",
                GradeId = GradeNames.MinBy(x => x.Ordinal)?.Id,
                AssessmentLevelGroupId = AssessmentLevelNames.MinBy(x => x.Ordinal)?.Id,
                IsTestAccount = false,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _studentService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetStudent(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _studentService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/student-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _studentService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task GenerateCode()
    {
        var response = await WithWaiting("Generating...", () => _studentService.GenerateSecretCode(new()));

        if (response.Ok)
            Entity!.SecretCode = response.Value.SecretCode;
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

    private void SetStudent(Student student)
    {
        Entity = student;
        _navigator.UpdateTitle(_path, Entity.Name!);
    }
}