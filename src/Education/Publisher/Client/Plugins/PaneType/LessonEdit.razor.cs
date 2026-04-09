using AchievementAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementAdded;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class LessonEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ILessonService LessonService { get; set; } = null!;

    public LessonEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var unitId = Path!.Id("unit");

        Model = new(Path, Id, IsNew, unitId, EventBus, Navigator, LessonService);
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

public class LessonEditModel : EditModel<Lesson>,
    IHandle<LessonSaved>, IHandle<LessonRemoved>, IHandle<LessonsReordered>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _unitId;
    private readonly INavigator _navigator;
    private readonly ILessonService _lessonService;

    public LessonEditModel(String? path, Guid? id, Boolean isNew, Guid? unitId,
        IEventBus eventBus,
        INavigator navigator,
        ILessonService lessonService) : base(isNew)
    {
        _path = path;
        _id = id;
        _unitId = unitId;
        _navigator = navigator;
        _lessonService = lessonService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(LessonSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(LessonRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(LessonsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _lessonService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title);
        }
    }

    public async Task Handle(AchievementAdded payload) => await FetchAchievementNames();

    public async Task Handle(AchievementSaved payload) => await FetchAchievementNames();

    public async Task Handle(AchievementRemoved payload) => await FetchAchievementNames();

    public ObservableCollection<Named> AchievementNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchAchievementNames(),
            FetchContentStatusNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var lesson = new Lesson
            {
                UnitId = _unitId,
                Title = "New Lesson",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                RequireSequentialCompletion = true,
                Treatment = true,
                Control = true,
            };

            SetLesson(lesson);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _lessonService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetLesson(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _lessonService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/lesson-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _lessonService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchAchievementNames()
    {
        var response = await WithAlerts(() => _lessonService.FetchAchievementNames(new()), false);
        if (response.Ok) AchievementNames = response.Value.ToObservable();
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _lessonService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    private void SetLesson(Lesson lesson)
    {
        Entity = lesson;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}