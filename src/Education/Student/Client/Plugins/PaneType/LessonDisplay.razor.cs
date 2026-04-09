using Crudspa.Content.Display.Client.Components;

namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class LessonDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public IObjectiveProgressService ObjectiveProgressService { get; set; } = null!;

    public LessonDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, SoundService, StudentAppService, ObjectiveProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class LessonDisplayModel : ScreenModel,
    IHandle<ObjectiveProgressUpdated>,
    IHandle<StudentAchievementAdded>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ISoundService _soundService;
    private readonly IStudentAppService _studentAppService;
    private readonly IObjectiveProgressService _objectiveProgressService;

    public LessonDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        ISoundService soundService,
        IStudentAppService studentAppService,
        IObjectiveProgressService objectiveProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _soundService = soundService;
        _studentAppService = studentAppService;
        _objectiveProgressService = objectiveProgressService;

        eventBus.Subscribe(this);
    }

    public Lesson? Lesson
    {
        get;
        set => SetProperty(ref field, value);
    }

    public GuideModel? GuideModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchLesson(new(new() { Id = _id })));

        if (response.Ok)
            await SetLesson(response.Value);
    }

    public Task Handle(ObjectiveProgressUpdated payload)
    {
        if (Lesson is null)
            return Task.CompletedTask;

        foreach (var objective in Lesson.Objectives)
            if (objective.Id.Equals(payload.Progress.ObjectiveId))
            {
                objective.Progress = payload.Progress;
                break;
            }

        EvaluateProgress();

        return Task.CompletedTask;
    }

    public async Task Handle(StudentAchievementAdded payload)
    {
        await Refresh();
    }

    public void GoToObjective(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/objective-{id:D}");
    }

    private void EvaluateProgress()
    {
        if (Lesson is null)
            return;

        if (Lesson.RequireSequentialCompletion == true)
        {
            var previousCompleted = true;
            foreach (var objective in Lesson.Objectives)
            {
                objective.Locked = !previousCompleted;
                previousCompleted = objective.Progress.TimesCompleted > 0;
            }
        }

        RaisePropertyChanged(nameof(Lesson));
    }

    private async Task SetLesson(Lesson lesson)
    {
        foreach (var objective in lesson.Objectives)
            objective.Progress = await _objectiveProgressService.Fetch(new(new() { Id = objective.Id }));

        Lesson = lesson;

        GuideModel = new()
        {
            Image = lesson.GuideImage,
            Text = lesson.GuideText,
            Audio = lesson.GuideAudio,
        };

        _navigator.UpdateTitle(_path, Lesson.Title!);

        EvaluateProgress();
    }
}