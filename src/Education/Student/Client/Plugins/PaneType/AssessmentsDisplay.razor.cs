using Crudspa.Education.Student.Client.Contracts.Events;

namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class AssessmentsDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IAssessmentRunService AssessmentRunService { get; set; } = null!;

    public AssessmentsDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, EventBus, Navigator, SoundService, AssessmentRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class AssessmentsDisplayModel : ScreenModel, IHandle<MadeProgress>
{
    private readonly String? _path;
    private readonly INavigator _navigator;
    private readonly ISoundService _soundService;
    private readonly IAssessmentRunService _assessmentRunService;

    public AssessmentsDisplayModel(String? path, IEventBus eventBus,
        INavigator navigator,
        ISoundService soundService,
        IAssessmentRunService assessmentRunService)
    {
        _path = path;
        _navigator = navigator;
        _soundService = soundService;
        _assessmentRunService = assessmentRunService;

        eventBus.Subscribe(this);
    }

    public List<AssessmentAssignment> Assessments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _assessmentRunService.FetchAssessments(new()));

        if (response.Ok)
            await SetAssessments(response.Value);
    }

    public async Task Handle(MadeProgress payload)
    {
        await Refresh();
    }

    public void GoToAssessment(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/assessment-{id:D}");
    }

    private async Task SetAssessments(IList<AssessmentAssignment> assessments)
    {
        Assessments = assessments.ToList();
        await Task.CompletedTask;
    }
}