namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class UnitsDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public IAssessmentRunService AssessmentRunService { get; set; } = null!;

    public UnitsDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Navigator, SoundService, StudentAppService, AssessmentRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class UnitsDisplayModel(String? path, INavigator navigator, ISoundService soundService, IStudentAppService studentAppService, IAssessmentRunService assessmentRunService)
    : ScreenModel, IHandle<StudentAchievementAdded>
{
    public ObservableCollection<Unit> Units
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<AssessmentAssignment> Assessments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Refresh()
    {
        await WithMany("Loading...",
            RefreshUnits(),
            RefreshAssessments());
    }

    public async Task RefreshUnits()
    {
        var response = await studentAppService.FetchUnits(new());

        if (response.Ok)
            Units = response.Value.ToObservable();
    }

    public async Task RefreshAssessments()
    {
        var response = await assessmentRunService.FetchAssessments(new());

        if (response.Ok)
            Assessments = response.Value.ToObservable();
    }

    public void GoToUnit(Guid? id)
    {
        soundService.ButtonPress();
        navigator.GoTo($"{path}/unit-{id:D}");
    }

    public void GoToAssessments()
    {
        soundService.ButtonPress();
        navigator.GoTo($"{path}/assessments");
    }

    public async Task Handle(StudentAchievementAdded payload)
    {
        await Refresh();
    }
}