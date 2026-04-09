using ShowAchievement = Crudspa.Education.Student.Client.Contracts.Events.ShowAchievement;

namespace Crudspa.Education.Student.Client.Modals;

public partial class TrophyCase : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public TrophyCaseModel Model { get; set; } = null!;

    [Inject] public ISessionState SessionState { get; set; } = null!;

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

public class TrophyCaseModel(
    IScrollService scrollService,
    IEventBus eventBus,
    IStudentAppService studentAppService)
    : ModalModel(scrollService)
{
    public ObservableCollection<StudentAchievement>? StudentAchievements
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override async Task Show()
    {
        await base.Show();

        var response = await WithWaiting("Working...", () => studentAppService.FetchAchievements(new()));

        if (response.Ok)
            StudentAchievements = response.Value.ToObservable();
    }

    public override Task Hide()
    {
        StudentAchievements = null;
        return base.Hide();
    }

    public void ViewAchievement(StudentAchievement studentAchievement)
    {
        studentAchievement.IsNew = false;
        eventBus.Publish(new ShowAchievement { StudentAchievement = studentAchievement });
    }
}