namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class Rearrange : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public RearrangeModel Model { get; set; } = null!;

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

public class RearrangeModel : Observable
{
    public event EventHandler<Guid>? Completed;

    private readonly IEventBus _eventBus;
    private readonly ISoundService _soundService;

    public Activity Activity { get; }
    public ObservableCollection<PieceModel> Options { get; set; } = null!;
    public ObservableCollection<ActivityChoice> Choices { get; set; }
    public ObservableCollection<DestinationModel> Destinations { get; set; }
    public String? ExtraText { get; set; }

    public RearrangeModel(IEventBus eventBus, ISoundService soundService, Activity activity)
    {
        _eventBus = eventBus;
        _soundService = soundService;

        Activity = activity;

        Choices = Activity.ActivityChoices;
        ExtraText = Activity.ExtraText;

        InitializeOptions();

        Destinations = Choices.OrderBy(x => x.Ordinal)
            .Select(x => new DestinationModel(x, null))
            .ToObservable();
    }

    public Boolean IsCompleted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean FirstTry
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public void InitializeOptions()
    {
        var options = Choices.Select(x => new PieceModel(x)).ToList();

        if (!String.IsNullOrEmpty(ExtraText))
        {
            var herrings = ExtraText.Split(",");
            foreach (var herring in herrings)
                options.Add(new(new() { Id = Guid.NewGuid(), Text = herring.Trim() }));
        }

        Options = options.Shuffle().ToObservable();
    }

    public void Reset()
    {
        if (IsCompleted)
        {
            IsCompleted = false;
            InitializeOptions();

            foreach (var destination in Destinations)
            {
                destination.PlacedPiece = null;
                destination.State = DestinationModel.States.Empty;
            }
        }
        else
        {
            foreach (var destination in Destinations)
            {
                if (destination.State == DestinationModel.States.Invalid)
                {
                    MovePieceBack(destination);
                    destination.PlacedPiece = null;
                    destination.State = DestinationModel.States.Empty;
                }
            }
        }

        Activity.Assignment.ActivityChoiceSelections = [];
    }

    public Task SelectOption(Guid id)
    {
        _soundService.ChoiceSelected();

        foreach (var option in Options)
        {
            option.State = option.Id.Equals(id) && option.State != PieceModel.States.Selected
                ? PieceModel.States.Selected
                : PieceModel.States.Default;
        }

        RaisePropertyChanged(nameof(Options));

        return Task.CompletedTask;
    }

    public async Task SelectDestination(Guid targetChoiceId)
    {
        _soundService.ChoiceSelected();

        var destination = Destinations.SingleOrDefault(x => x.TargetPiece!.Id == targetChoiceId);

        if (destination is null
            || destination.State == DestinationModel.States.Valid
            || destination.State == DestinationModel.States.Invalid)
            return;

        if (destination.State == DestinationModel.States.Filled)
        {
            MovePieceBack(destination);
            return;
        }

        var selectedOption = Options.SingleOrDefault(x => x.State == PieceModel.States.Selected);
        if (selectedOption is null) return;

        destination.PlacedPiece = selectedOption;
        destination.State = DestinationModel.States.Filled;

        RecordSelection(selectedOption.Id!.Value, destination.TargetPiece!.Id!.Value);

        Options.Remove(selectedOption);

        RaisePropertyChanged(nameof(Options));

        await EvaluateProgress();
    }

    public void MovePieceBack(DestinationModel destination)
    {
        destination.PlacedPiece!.State = PieceModel.States.Default;

        Options.Add(destination.PlacedPiece);

        destination.PlacedPiece = null;
        destination.State = DestinationModel.States.Empty;

        RaisePropertyChanged(nameof(Options));
    }

    public async Task EvaluateProgress()
    {
        await _eventBus.Publish(new SilenceRequested());

        if (Destinations.All(x => x.PlacedPiece is not null))
        {
            foreach (var destination in Destinations)
            {
                destination.State = destination.PlacedPiece!.Id.Equals(destination.TargetPiece!.Id)
                    ? DestinationModel.States.Valid
                    : DestinationModel.States.Invalid;
            }

            if (Destinations.All(x => x.State == DestinationModel.States.Valid))
            {
                _soundService.ChoiceCorrect();

                RaisePropertyChanged(nameof(Options));

                await Task.Delay(750);
                RaiseCompleted(ActivityAssignmentStatusIds.Successful);
                IsCompleted = true;
            }
            else
            {
                _soundService.ChoiceIncorrect();

                RaisePropertyChanged(nameof(Options));

                await Task.Delay(2250);

                if (FirstTry)
                {
                    FirstTry = false;

                    foreach (var destination in Destinations)
                    {
                        if (destination.State == DestinationModel.States.Valid)
                            destination.State = DestinationModel.States.Filled;
                        else if (destination.State == DestinationModel.States.Invalid)
                            MovePieceBack(destination);
                    }
                }
                else
                    RaiseCompleted(ActivityAssignmentStatusIds.Failed);
            }
        }
    }

    private void RecordSelection(Guid choiceId, Guid targetChoiceId)
    {
        // Red herrings use an empty guid for the ID that gets saved
        var safeChoiceId = Options.Any(x => x.Id == choiceId)
            ? choiceId
            : Guid.Empty;

        Activity.Assignment.ActivityChoiceSelections.Add(new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = Activity.Assignment.Id,
            ChoiceId = safeChoiceId,
            TargetChoiceId = targetChoiceId,
            Made = DateTime.Now,
        });
    }

    private void RaiseCompleted(Guid status)
    {
        var raiseEvent = Completed;
        raiseEvent?.Invoke(this, status);
    }
}