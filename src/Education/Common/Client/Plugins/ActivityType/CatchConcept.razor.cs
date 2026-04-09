namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class CatchConcept : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public CatchConceptModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, Activity!);
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

public class CatchConceptModel : CatchModel
{
    public event EventHandler<Guid>? Completed;

    private readonly IEventBus _eventBus;

    public CatchConceptModel(IEventBus eventBus, Activity activity)
    {
        _eventBus = eventBus;

        Activity = activity;

        foreach (var activityChoice in activity.ActivityChoices.Shuffle())
            Options.Add(new(activityChoice));

        Reset();
    }

    public void Reset()
    {
        foreach (var option in Options)
            option.State = ActivityChoiceModel.States.Default;

        Activity.Assignment.ActivityChoiceSelections = [];
    }

    public Activity Activity { get; }

    public Task Start()
    {
        BounceAround();
        return Task.CompletedTask;
    }

    public void Stop()
    {
        StopBouncing();
        Silence();
        Reset();
    }

    public async Task SelectChoice(Guid choiceId)
    {
        if (Bouncing)
        {
            var option = SelectOption(choiceId);

            if (option.Choice.AudioFile?.Id is not null && option.AudioPlayerElement is not null)
                await option.AudioPlayerElement.PlayAndWait();

            await EvaluateProgress();
        }
    }

    private ActivityChoiceModel SelectOption(Guid id)
    {
        var option = Options.FirstOrDefault(x => x.Choice.Id == id);

        if (option!.State == ActivityChoiceModel.States.Default)
        {
            option.State = option.Choice.IsCorrect == true
                ? ActivityChoiceModel.States.Valid
                : ActivityChoiceModel.States.Invalid;
        }

        RecordSelection(id);

        return option;
    }

    private void RecordSelection(Guid id)
    {
        Activity.Assignment.ActivityChoiceSelections.Add(new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = Activity.Assignment.Id,
            ChoiceId = id,
            Made = DateTimeOffset.Now,
        });
    }

    private void Silence()
    {
        _eventBus.Publish(new SilenceRequested());
    }

    private async Task EvaluateProgress()
    {
        // All correct answers must be selected before we consider the activity completed
        if (!Options.Any(x => x.Choice.IsCorrect == true && x.State != ActivityChoiceModel.States.Valid))
        {
            await Task.Delay(1000);

            // If any incorrect answers were selected, consider it a failure
            var status = Options.Any(x => x.State == ActivityChoiceModel.States.Invalid)
                ? ActivityAssignmentStatusIds.Failed
                : ActivityAssignmentStatusIds.Successful;

            RaiseCompleted(status);

            Stop();
        }
    }

    private void RaiseCompleted(Guid status)
    {
        var raiseEvent = Completed;
        raiseEvent?.Invoke(this, status);
    }
}