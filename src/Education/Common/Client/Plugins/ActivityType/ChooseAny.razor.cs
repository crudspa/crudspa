namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class ChooseAny : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public ChooseAnyModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, SoundService, Activity!);
        Model.PropertyChanged += HandleModelChanged;
        Model.Completed += HandleCompleted;

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Completed -= HandleCompleted;
    }

    public void Reset() => Model.Reset();

    private async Task HandleContextAudioPlayed(MediaPlay mediaPlay)
    {
        var activityMediaPlay = new ActivityMediaPlay
        {
            MediaPlay = mediaPlay,
            ActivityId = Activity?.Id,
            AssignmentBatchId = AssignmentBatchId,
        };

        await ActivityMediaPlayService.Add(new(activityMediaPlay));
    }

    public async Task HandleChoiceAudioPlayed(ActivityChoiceModel option, MediaPlay mediaPlay)
    {
        var activityMediaPlay = new ActivityMediaPlay
        {
            MediaPlay = mediaPlay,
            ActivityId = Activity?.Id,
            AssignmentBatchId = AssignmentBatchId,
            ActivityChoiceId = option.Choice.Id,
        };

        await ActivityMediaPlayService.Add(new(activityMediaPlay));
    }
}

public class ChooseAnyModel : ChoicesModel
{
    public event EventHandler<Guid>? Completed;

    private readonly IEventBus _eventBus;
    private readonly ISoundService _soundService;

    public ChooseAnyModel(IEventBus eventBus, ISoundService soundService, Activity activity)
    {
        _eventBus = eventBus;
        _soundService = soundService;

        Activity = activity;

        if (activity.ActivityTypeShuffleChoices == true)
            foreach (var activityChoice in activity.ActivityChoices.Shuffle())
                Options.Add(new(activityChoice));
        else
            foreach (var activityChoice in activity.ActivityChoices)
                Options.Add(new(activityChoice));
    }

    public void Reset()
    {
        if (Activity.ActivityTypeShuffleChoices == true)
            Options = Options.Shuffle().ToObservable();

        foreach (var option in Options)
            option.State = ActivityChoiceModel.States.Default;

        Activity.Assignment.ActivityChoiceSelections = [];
    }

    public Activity Activity { get; }

    public async Task SelectOption(Guid? id)
    {
        _soundService.ChoiceSelected();

        await _eventBus.Publish(new SilenceRequested());

        var option = Options.First(x => x.Choice.Id.Equals(id));

        if (option.State != ActivityChoiceModel.States.Default)
            return;

        RecordSelection(id);

        option.State = ActivityChoiceModel.States.Selected;

        foreach (var choiceModel in Options.Where(x => x.State == ActivityChoiceModel.States.Default))
            choiceModel.State = ActivityChoiceModel.States.Locked;

        RaisePropertyChanged(nameof(Options));

        await Task.Delay(1250);

        RaiseCompleted(ActivityAssignmentStatusIds.Successful);
    }

    private void RecordSelection(Guid? id)
    {
        Activity.Assignment.ActivityChoiceSelections.Add(new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = Activity.Assignment.Id,
            ChoiceId = id,
            Made = DateTimeOffset.Now,
        });
    }

    private void RaiseCompleted(Guid status)
    {
        var raiseEvent = Completed;
        raiseEvent?.Invoke(this, status);
    }
}