namespace Crudspa.Education.School.Client.Plugins.PaneType;

public partial class ClassroomEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IClassroomService ClassroomService { get; set; } = null!;

    public ClassroomEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, ClassroomService);
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

public class ClassroomEditModel : EditModel<Classroom>, IHandle<ClassroomSaved>, IHandle<ClassroomRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IClassroomService _classroomService;

    public ModalModel AddTeachersModel { get; set; }
    public ModalModel AddStudentsModel { get; set; }

    public ClassroomEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService,
        IClassroomService classroomService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _classroomService = classroomService;

        AddTeachersModel = new(scrollService);
        AddStudentsModel = new(scrollService);

        eventBus.Subscribe(this);
    }

    public async Task Handle(ClassroomSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ClassroomRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<Orderable> TypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> GradeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> SelectableTeachers
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> SelectableStudents
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchTypeNames(),
            FetchGradeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetClassroom(new()
            {
                OrganizationName = "New Classroom",
                TypeId = TypeNames.MinBy(x => x.Ordinal)?.Id,
                GradeId = GradeNames.MinBy(x => x.Ordinal)?.Id,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _classroomService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetClassroom(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _classroomService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/classroom-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _classroomService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchTypeNames()
    {
        var response = await WithAlerts(() => _classroomService.FetchTypeNames(new()), false);
        if (response.Ok) TypeNames = response.Value.ToList();
    }

    public async Task FetchGradeNames()
    {
        var response = await WithAlerts(() => _classroomService.FetchGradeNames(new()), false);
        if (response.Ok) GradeNames = response.Value.ToList();
    }

    public async Task ShowSelectableTeachers()
    {
        var response = await WithWaiting("Fetching contacts...", () => _classroomService.FetchSchoolContacts(new(new() { Id = _id })));

        if (response.Ok)
        {
            var teachers = response.Value;
            teachers.RemoveWhere(x => Entity!.ClassroomTeachers.HasAny(y => y.SchoolContactId.Equals(x.Id)));
            SelectableTeachers = teachers.ToObservable();
        }

        await AddTeachersModel.Show();
    }

    public void AddTeachers()
    {
        AddTeachersModel.Hide();

        foreach (var teacher in SelectableTeachers.Where(x => x.Selected == true))
        {
            Entity!.ClassroomTeachers.Add(new()
            {
                ClassroomId = Entity.Id,
                SchoolContactId = teacher.Id,
                Name = teacher.Name,
            });
        }
    }

    public void RemoveTeacher(Guid? teacherId)
    {
        Entity!.ClassroomTeachers.RemoveWhere(x => x.Id.Equals(teacherId));
    }

    public async Task ShowSelectableStudents()
    {
        var response = await WithWaiting("Fetching students...", () => _classroomService.FetchStudents(new(new() { Id = _id })));

        if (response.Ok)
        {
            var students = response.Value;
            students.RemoveWhere(x => Entity!.ClassroomStudents.HasAny(y => y.StudentId.Equals(x.Id)));
            SelectableStudents = students.ToObservable();
        }

        await AddStudentsModel.Show();
    }

    public void AddStudents()
    {
        AddStudentsModel.Hide();

        foreach (var teacher in SelectableStudents.Where(x => x.Selected == true))
        {
            Entity!.ClassroomStudents.Add(new()
            {
                ClassroomId = Entity.Id,
                StudentId = teacher.Id,
                Name = teacher.Name,
            });
        }
    }

    public void RemoveStudent(Guid? teacherId)
    {
        Entity!.ClassroomStudents.RemoveWhere(x => x.Id.Equals(teacherId));
    }

    private void SetClassroom(Classroom classroom)
    {
        Entity = classroom;
        _navigator.UpdateTitle(_path, Entity.Name!);
    }
}