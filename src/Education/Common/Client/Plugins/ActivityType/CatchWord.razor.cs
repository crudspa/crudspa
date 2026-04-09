namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class CatchWord : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public CatchWordModel Model { get; set; } = null!;

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
}

public class CatchWordModel : CatchModel
{
    public event EventHandler<Guid>? Completed;

    private readonly IEventBus _eventBus;
    private readonly ISoundService _soundService;
    private List<ActivityChoiceModel> _correctOrder = [];
    private ActivityChoiceModel? _activeOption;
    private Timer? _speakerTimer;

    public Activity Activity { get; }

    public CatchWordModel(IEventBus eventBus, ISoundService soundService, Activity activity)
    {
        _eventBus = eventBus;
        _soundService = soundService;

        Activity = activity;

        foreach (var activityChoice in activity.ActivityChoices.Shuffle())
            Options.Add(new(activityChoice));

        Reset();
    }

    public void Reset()
    {
        _activeOption = null;
        _correctOrder = Options.Shuffle().ToList();

        foreach (var option in Options)
            option.State = ActivityChoiceModel.States.Default;

        Activity.Assignment.ActivityChoiceSelections = [];
    }

    public async Task Start()
    {
        BounceAround();

        if (ActivateNextOption())
            await Speak();
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
            var state = SelectOption(choiceId);

            if (state == ActivityChoiceModel.States.Valid)
            {
                _soundService.ChoiceCorrect();
                await Task.Delay(750);

                if (ActivateNextOption())
                    await Speak();

                Unlock();
            }
            else if (state == ActivityChoiceModel.States.Invalid)
            {
                _soundService.ChoiceIncorrect();
                await Task.Delay(2250);

                Unlock();
            }

            EvaluateProgress();
        }
    }

    private ActivityChoiceModel.States SelectOption(Guid id)
    {
        var option = Options.First(x => x.Choice.Id.Equals(id));

        if (option.State != ActivityChoiceModel.States.Default)
            return ActivityChoiceModel.States.Locked;

        option.State = _activeOption?.Choice.Id == id
            ? ActivityChoiceModel.States.Valid
            : ActivityChoiceModel.States.Invalid;

        foreach (var o in Options.Where(x => x.State == ActivityChoiceModel.States.Default))
            o.State = ActivityChoiceModel.States.Locked;

        RecordSelection(id);

        return option.State;
    }

    private void RecordSelection(Guid id)
    {
        Activity.Assignment.ActivityChoiceSelections.Add(new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = Activity.Assignment.Id,
            ChoiceId = id,
            TargetChoiceId = _activeOption?.Choice.Id ?? Guid.Empty,
            Made = DateTimeOffset.Now,
        });
    }

    private Boolean ActivateNextOption()
    {
        if (_activeOption is not null)
            _correctOrder = _correctOrder.Skip(1).ToList();

        if (_correctOrder.Any())
        {
            _activeOption = Options.FirstOrDefault(x => x.Choice.Id == _correctOrder.First().Choice.Id);
            return true;
        }

        return false;
    }

    private async Task Speak()
    {
        Silence();

        if (Bouncing && _activeOption?.AudioPlayerElement is not null)
        {
            _speakerTimer = new(_ => Task.Run(Speak), null, 5000, Timeout.Infinite);
            await _activeOption.AudioPlayerElement.Play();
        }
    }

    private void Silence()
    {
        _speakerTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _speakerTimer = null;

        _eventBus.Publish(new SilenceRequested());
    }

    private void Unlock()
    {
        foreach (var option in Options)
            option.State = ActivityChoiceModel.States.Default;
    }

    private void EvaluateProgress()
    {
        var wrongAnswers = Activity.Assignment.ActivityChoiceSelections.Count(x => !x.ChoiceId.Equals(x.TargetChoiceId));

        if (wrongAnswers >= Options.Count || _correctOrder.Count == 0)
        {
            var status = wrongAnswers < Options.Count
                ? ActivityAssignmentStatusIds.Successful
                : ActivityAssignmentStatusIds.Failed;

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