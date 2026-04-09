namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class TableSort : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public TableSortModel Model { get; set; } = null!;

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

public class TableSortModel : Observable
{
    public event EventHandler<Guid>? Completed;

    private readonly IEventBus _eventBus;
    private readonly ISoundService _soundService;

    public Activity Activity { get; }
    public ObservableCollection<ActivityChoice> Choices { get; set; }
    public ObservableCollection<PieceModel> Options { get; set; } = [];
    public ObservableCollection<DestinationModel> ColumnADestinations { get; set; }
    public ObservableCollection<DestinationModel> ColumnBDestinations { get; set; }
    public String ColumnATitle { get; set; } = "Column A";
    public String ColumnBTitle { get; set; } = "Column B";

    public TableSortModel(IEventBus eventBus, ISoundService soundService, Activity activity)
    {
        _eventBus = eventBus;
        _soundService = soundService;

        Activity = activity;

        Choices = Activity.ActivityChoices;

        InitializeOptions();

        ColumnADestinations = new(Choices
            .Where(x => x.ColumnOrdinal == 0)
            .OrderBy(x => x.Ordinal)
            .Select(x => new DestinationModel(x, null)));

        ColumnBDestinations = new(Choices
            .Where(x => x.ColumnOrdinal != 0)
            .OrderBy(x => x.Ordinal)
            .Select(x => new DestinationModel(x, null)));

        var separatorPosition = Activity.ExtraText!.IndexOf("|", StringComparison.Ordinal);
        if (separatorPosition > 0)
        {
            ColumnATitle = Activity.ExtraText.Substring(0, separatorPosition);
            ColumnBTitle = Activity.ExtraText.Substring(separatorPosition + 1);
        }
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

    public void Reset()
    {
        InitializeOptions();

        foreach (var destination in ColumnADestinations)
        {
            destination.PlacedPiece = null;
            destination.State = DestinationModel.States.Empty;
        }

        foreach (var destination in ColumnBDestinations)
        {
            destination.PlacedPiece = null;
            destination.State = DestinationModel.States.Empty;
        }

        Activity.Assignment.ActivityChoiceSelections = [];
    }

    public void InitializeOptions()
    {
        Options = new(Choices.Shuffle().Select(x => new PieceModel(x)));
    }

    public Task SelectOption(Guid id)
    {
        _soundService.ChoiceSelected();

        foreach (var option in Options)
        {
            option.State = option.Id == id && option.State != PieceModel.States.Selected
                ? PieceModel.States.Selected
                : PieceModel.States.Default;
        }

        RaisePropertyChanged(nameof(Options));

        return Task.CompletedTask;
    }

    public async Task SelectColumnADestination(Guid targetChoiceId)
    {
        var destination = ColumnADestinations.SingleOrDefault(x => x.TargetPiece!.Id == targetChoiceId);
        await SelectDestination(destination);
    }

    public async Task SelectColumnBDestination(Guid targetChoiceId)
    {
        var destination = ColumnBDestinations.SingleOrDefault(x => x.TargetPiece!.Id == targetChoiceId);
        await SelectDestination(destination);
    }

    public async Task SelectDestination(DestinationModel? destination)
    {
        _soundService.ChoiceSelected();

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

        Options.Remove(selectedOption);

        RaisePropertyChanged(nameof(Options));

        await EvaluateProgress();
        RecordSelection(selectedOption.Id!.Value, destination.TargetPiece!.Id!.Value);
    }

    private void MovePieceBack(DestinationModel destination)
    {
        destination.PlacedPiece!.State = PieceModel.States.Default;

        Options.Add(destination.PlacedPiece);

        destination.PlacedPiece = null;
        destination.State = DestinationModel.States.Empty;
    }

    public async Task EvaluateProgress()
    {
        await _eventBus.Publish(new SilenceRequested());

        if (ColumnADestinations.All(x => x.PlacedPiece is not null)
            && ColumnBDestinations.All(x => x.PlacedPiece is not null))
        {
            foreach (var destination in ColumnADestinations)
            {
                destination.State = ColumnADestinations.Any(x => x.TargetPiece!.Id == destination.PlacedPiece!.Id)
                    ? DestinationModel.States.Valid
                    : DestinationModel.States.Invalid;
            }

            foreach (var destination in ColumnBDestinations)
            {
                destination.State = ColumnBDestinations.Any(x => x.TargetPiece!.Id == destination.PlacedPiece!.Id)
                    ? DestinationModel.States.Valid
                    : DestinationModel.States.Invalid;
            }

            if (ColumnADestinations.All(x => x.State == DestinationModel.States.Valid)
                && ColumnBDestinations.All(x => x.State == DestinationModel.States.Valid))
            {
                _soundService.ChoiceCorrect();
                RaisePropertyChanged(nameof(Options));

                await Task.Delay(750);

                RaiseCompleted(ActivityAssignmentStatusIds.Successful);
            }
            else
            {
                _soundService.ChoiceIncorrect();
                RaisePropertyChanged(nameof(Options));

                await Task.Delay(2250);

                if (FirstTry)
                {
                    FirstTry = false;

                    foreach (var destination in ColumnADestinations)
                    {
                        if (destination.State == DestinationModel.States.Valid)
                            destination.State = DestinationModel.States.Filled;
                        else if (destination.State == DestinationModel.States.Invalid)
                            MovePieceBack(destination);
                    }

                    foreach (var destination in ColumnBDestinations)
                    {
                        if (destination.State == DestinationModel.States.Valid)
                            destination.State = DestinationModel.States.Filled;
                        else if (destination.State == DestinationModel.States.Invalid)
                            MovePieceBack(destination);
                    }

                    RaisePropertyChanged(nameof(Options));
                }
                else
                    RaiseCompleted(ActivityAssignmentStatusIds.Failed);
            }
        }
    }

    private void RecordSelection(Guid choiceId, Guid targetChoiceId)
    {
        Activity.Assignment.ActivityChoiceSelections.Add(new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = Activity.Assignment.Id,
            ChoiceId = choiceId,
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