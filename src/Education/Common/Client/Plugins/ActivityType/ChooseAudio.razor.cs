namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class ChooseAudio : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public ChooseAudioModel Model { get; set; } = null!;

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

public class ChooseAudioModel : ChoicesModel
{
    public event EventHandler<Guid>? Completed;

    private readonly IEventBus _eventBus;
    private readonly ISoundService _soundService;
    private readonly Int32 _maxIncorrectSelections;

    public ChooseAudioModel(IEventBus eventBus, ISoundService soundService, Activity activity)
    {
        _eventBus = eventBus;
        _soundService = soundService;

        Activity = activity;

        foreach (var activityChoice in activity.ActivityChoices.Shuffle())
            Options.Add(new(activityChoice));

        _maxIncorrectSelections = Options.Count <= 2 ? 1 : 2;
    }

    public void Reset()
    {
        foreach (var option in Options)
            option.State = ActivityChoiceModel.States.Default;

        Activity.Assignment.ActivityChoiceSelections = [];
    }

    public Activity Activity { get; }

    public async Task SelectOption(Guid? id)
    {
        var option = Options.First(x => x.Choice.Id.Equals(id));

        if (option.State != ActivityChoiceModel.States.Default)
            return;

        RecordSelection(id);

        option.State = option.Choice.IsCorrect == true
            ? ActivityChoiceModel.States.Valid
            : ActivityChoiceModel.States.Invalid;

        foreach (var choiceModel in Options.Where(x => x.State == ActivityChoiceModel.States.Default))
            choiceModel.State = ActivityChoiceModel.States.Locked;

        RaisePropertyChanged(nameof(Options));

        if (option.Choice.AudioFile?.Id is not null && option.AudioPlayerElement is not null)
            await option.AudioPlayerElement.PlayAndWait();

        await EvaluateProgress();
    }

    public async Task EvaluateProgress()
    {
        var invalidSelections = Options.Count(x => x.State == ActivityChoiceModel.States.Invalid);
        var unselectedValidOptions = Options.Count(x => x.Choice.IsCorrect == true && x.State != ActivityChoiceModel.States.Valid);

        if (invalidSelections >= _maxIncorrectSelections)
        {
            await _eventBus.Publish(new SilenceRequested());
            RaiseCompleted(ActivityAssignmentStatusIds.Failed);
        }
        else if (unselectedValidOptions == 0)
        {
            await Task.Delay(1250);
            RaiseCompleted(ActivityAssignmentStatusIds.Successful);
        }

        Unlock();
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

    private void Unlock()
    {
        foreach (var option in Options.Where(x => x.State == ActivityChoiceModel.States.Locked))
            option.State = ActivityChoiceModel.States.Default;
    }

    private void RaiseCompleted(Guid status)
    {
        var raiseEvent = Completed;
        raiseEvent?.Invoke(this, status);
    }
}