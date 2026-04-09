namespace Crudspa.Education.Student.Client.Modals;

public partial class StudentAchievementModal : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public AchievementModel Model { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class AchievementModel(
    IScrollService scrollService,
    INavigator navigator,
    ISoundService soundService,
    IStudentAppService studentAppService)
    : ModalModel(scrollService)
{
    private Guid? _id;
    private Boolean _isNew;

    public StudentAchievement? StudentAchievement
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Show(StudentAchievement studentAchievement)
    {
        _id = studentAchievement.Id;
        _isNew = studentAchievement.IsNew;

        await Show();
    }

    public override async Task Show()
    {
        if (_id is null)
            return;

        await base.Show();

        var response = await WithWaiting("Working...", () => studentAppService.FetchAchievement(new(new() { Id = _id })));

        if (response.Ok)
        {
            var achievement = response.Value;
            achievement.IsNew = _isNew;
            StudentAchievement = achievement;
        }
    }

    public override Task Hide()
    {
        StudentAchievement = null;
        return base.Hide();
    }

    public Boolean HasUnlocks
    {
        get
        {
            if (StudentAchievement is null)
                return false;

            return StudentAchievement.Unlocks.Books.Count > 0
                || StudentAchievement.Unlocks.Games.Count > 0
                || StudentAchievement.Unlocks.Lessons.Count > 0
                || StudentAchievement.Unlocks.Modules.Count > 0
                || StudentAchievement.Unlocks.Objectives.Count > 0
                || StudentAchievement.Unlocks.Trifolds.Count > 0
                || StudentAchievement.Unlocks.Units.Count > 0;
        }
    }

    public void GoToBook(Guid? unitId, Guid? bookId)
    {
        soundService.ButtonPress();
        navigator.GoTo($"/units/unit-{unitId:D}/book-{bookId:D}");
        Hide();
    }

    public void GoToGame(Guid? unitId, Guid? bookId, Guid? gameId)
    {
        soundService.ButtonPress();
        navigator.GoTo($"/units/unit-{unitId:D}/book-{bookId:D}/game-{gameId:D}");
        Hide();
    }

    public void GoToLesson(Guid? unitId, Guid? lessonId)
    {
        soundService.ButtonPress();
        navigator.GoTo($"/units/unit-{unitId:D}/lesson-{lessonId:D}");
        Hide();
    }

    public void GoToModule(Guid? unitId, Guid? bookId, Guid? moduleId)
    {
        soundService.ButtonPress();
        navigator.GoTo($"/units/unit-{unitId:D}/book-{bookId:D}/module-{moduleId:D}");
        Hide();
    }

    public void GoToObjective(Guid? unitId, Guid? lessonId, Guid? objectiveId)
    {
        soundService.ButtonPress();
        navigator.GoTo($"/units/unit-{unitId:D}/lesson-{lessonId:D}/objective-{objectiveId:D}");
        Hide();
    }

    public void GoToTrifold(Guid? unitId, Guid? bookId, Guid? trifoldId)
    {
        soundService.ButtonPress();
        navigator.GoTo($"/units/unit-{unitId:D}/book-{bookId:D}/trifold-{trifoldId:D}");
        Hide();
    }

    public void GoToUnit(Guid? unitId)
    {
        soundService.ButtonPress();
        navigator.GoTo($"/units/unit-{unitId:D}");
        Hide();
    }
}