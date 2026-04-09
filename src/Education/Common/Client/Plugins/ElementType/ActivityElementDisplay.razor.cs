using Crudspa.Education.Common.Shared.Contracts.Config.ElementType;

namespace Crudspa.Education.Common.Client.Plugins.ElementType;

public partial class ActivityElementDisplay : IElementDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IActivityRunService ActivityRunService { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;

    public ActivityElementDisplayModel Model { get; set; } = null!;
    public ActivityDisplayPlugin ActivityDisplay { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var activityElement = ElementModel.RequireConfig<ActivityElement>();

        activityElement.Activity!.Assignment = new()
        {
            Id = Guid.NewGuid(),
            ActivityId = activityElement.Activity.Id,
            Assigned = DateTimeOffset.Now,
            Started = DateTimeOffset.Now,
            StatusId = ActivityAssignmentStatusIds.Started,
        };

        Model = new(ScrollService, ActivityRunService, ElementModel, activityElement);
        Model.PropertyChanged += HandleModelChanged;

        await Model.ElementModel.InitializeProgress();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
    }

    public void PlayAgain()
    {
        (ActivityDisplay.Instance as IActivityDisplay)?.Reset();
        Model.ElementModel.Replaying = true;
        Model.ModalModel.Hide();
    }
}

public class ActivityElementDisplayModel(
    IScrollService scrollService,
    IActivityRunService activityRunService,
    ElementDisplayModel elementModel,
    ActivityElement activityElement)
    : Observable
{
    public ActivityElement ActivityElement { get; } = activityElement;
    public ActivityElementModalModel ModalModel { get; } = new(scrollService);
    public ElementDisplayModel ElementModel { get; } = elementModel;

    public async Task HandleActivityCompleted(Guid status)
    {
        if (status.Equals(ActivityAssignmentStatusIds.Successful))
        {
            ModalModel.Correct = true;
            await ElementModel.AddElementCompleted();
        }
        else
        {
            ModalModel.Correct = false;
            ElementModel.MarkElementIncorrect();
        }

        await ModalModel.Show();

        var assignment = ActivityElement.Activity!.Assignment;
        assignment.StatusId = status;
        assignment.Finished = DateTimeOffset.Now;

        await activityRunService.AddActivityState(new(assignment));
    }
}

public class ActivityElementModalModel(IScrollService scrollService) : ModalModel(scrollService)
{
    public Boolean Correct
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String Message => Correct ? "Activity Completed!" : "Incorrect.";
    public String ButtonText => Correct ? "Play Again" : "Try Again";
}