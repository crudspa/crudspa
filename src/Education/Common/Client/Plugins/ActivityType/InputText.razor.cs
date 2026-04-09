namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class InputText : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public InputTextModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Activity!);
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

public class InputTextModel : Observable
{
    public event EventHandler<Guid>? Completed;

    public InputTextModel(Activity activity)
    {
        Activity = activity;

        var existing = activity.Assignment.TextEntries
            .Where(x => x.AssignmentId.Equals(activity.Assignment.Id))
            .OrderByDescending(x => x.Made)
            .FirstOrDefault();

        if (existing is not null && existing.Text.HasSomething())
            TextInput = existing.Text;
    }

    public void Reset()
    {
        Saved = false;
    }

    public Activity Activity { get; }

    public String TextInput
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public Boolean Saved
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void RecordTextInput()
    {
        if (TextInput.HasNothing())
            return;

        Saved = true;

        Activity.Assignment.TextEntries.Add(new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = Activity.Assignment.Id,
            Made = DateTime.Now,
            Text = TextInput,
            Ordinal = 0,
        });

        RaiseCompleted(ActivityAssignmentStatusIds.Successful);
    }

    public void EditTextInput()
    {
        Saved = false;
    }

    private void RaiseCompleted(Guid status)
    {
        var raiseEvent = Completed;
        raiseEvent?.Invoke(this, status);
    }
}