namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class Joke : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public JokeModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(SoundService, Activity!);
        Model.PropertyChanged += HandleModelChanged;
        Model.Completed += HandleCompleted;

        await Task.CompletedTask;
    }

    public void Reset() { }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Completed -= HandleCompleted;
    }

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

public class JokeModel(ISoundService soundService, Activity activity)
    : Observable
{
    public event EventHandler<Guid>? Completed;

    public Activity Activity { get; } = activity;

    public Boolean Revealed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Reveal()
    {
        soundService.ButtonPress();

        Revealed = true;

        await Task.Delay(4000);

        RaiseCompleted(ActivityAssignmentStatusIds.Successful);
    }

    private void RaiseCompleted(Guid status)
    {
        var raiseEvent = Completed;
        raiseEvent?.Invoke(this, status);
    }
}