namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class ConceptMap : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public ConceptMapModel Model { get; set; } = null!;

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

public class ConceptMapModel : Observable
{
    public event EventHandler<Guid>? Completed;

    private readonly IEventBus _eventBus;
    private readonly ISoundService _soundService;

    public Activity Activity { get; }
    public ObservableCollection<PieceModel> AnswerBank { get; set; } = [];
    public ObservableCollection<ConceptChoiceModel> ConceptChoices { get; set; } = [];

    public ConceptMapModel(IEventBus eventBus, ISoundService soundService, Activity activity)
    {
        _eventBus = eventBus;
        _soundService = soundService;

        Activity = activity;

        InitializeOptions();
    }

    public Boolean FirstTry
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Boolean IsCompleted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void Reset()
    {
        if (IsCompleted)
        {
            IsCompleted = false;
            InitializeOptions();

            foreach (var conceptChoice in ConceptChoices)
            {
                var destination = conceptChoice.Destination;
                destination!.PlacedPiece = null;
                destination.State = DestinationModel.States.Empty;
            }
        }
        else
        {
            foreach (var conceptChoice in ConceptChoices)
            {
                if (conceptChoice.Destination!.State == DestinationModel.States.Invalid)
                {
                    MovePieceBack(conceptChoice.Destination);
                    conceptChoice.Destination.PlacedPiece = null;
                    conceptChoice.Destination.State = DestinationModel.States.Empty;
                }
            }
        }

        Activity.Assignment.ActivityChoiceSelections = [];
    }

    public void InitializeOptions()
    {
        ConceptChoices = new(Activity.ActivityChoices.Select(choice => new ConceptChoiceModel(choice)));

        var answers = ConceptChoices
            .Select(x => new PieceModel(new()
            {
                Id = x.Choice.Id,
                Text = x.Destination!.TargetPiece!.Text,
            })).ToList();

        if (!String.IsNullOrEmpty(Activity.ExtraText))
            foreach (var herring in Activity.ExtraText.Split(","))
                answers.Add(new(new() { Id = Guid.NewGuid(), Text = herring.Trim() }));

        AnswerBank = answers.Shuffle().ToObservable();
    }

    public Task SelectOption(Guid id)
    {
        _soundService.ChoiceSelected();

        foreach (var option in AnswerBank)
        {
            option.State = option.Id.Equals(id) && option.State != PieceModel.States.Selected
                ? PieceModel.States.Selected
                : PieceModel.States.Default;
        }

        return Task.CompletedTask;
    }

    public async Task SelectDestination(Guid targetChoiceId)
    {
        _soundService.ChoiceSelected();

        var destination = ConceptChoices.FirstOrDefault(x => x.Destination is not null && x.Destination.TargetPiece!.Id == targetChoiceId)?.Destination;

        if (destination is null
            || destination.State == DestinationModel.States.Valid
            || destination.State == DestinationModel.States.Invalid)
            return;

        if (destination.State == DestinationModel.States.Filled)
        {
            MovePieceBack(destination);
            return;
        }

        var selectedOption = AnswerBank.FirstOrDefault(x => x.State == PieceModel.States.Selected);
        if (selectedOption is null) return;

        destination.PlacedPiece = selectedOption;
        destination.State = DestinationModel.States.Filled;

        RecordSelection(selectedOption.Id!.Value, destination.TargetPiece!.Id!.Value);

        AnswerBank.Remove(selectedOption);

        await EvaluateProgress();
    }

    private void MovePieceBack(DestinationModel destination)
    {
        destination.PlacedPiece!.State = PieceModel.States.Default;

        AnswerBank.Add(destination.PlacedPiece);

        destination.PlacedPiece = null;
        destination.State = DestinationModel.States.Empty;
    }

    public async Task EvaluateProgress()
    {
        await _eventBus.Publish(new SilenceRequested());

        if (ConceptChoices.All(x => x.Destination!.PlacedPiece is not null))
        {
            foreach (var choice in ConceptChoices)
            {
                choice.Destination!.State = choice.Destination.PlacedPiece!.Id == choice.Destination.TargetPiece!.Id
                    ? DestinationModel.States.Valid
                    : DestinationModel.States.Invalid;
            }

            if (ConceptChoices.All(x => x.Destination is not null && x.Destination.State == DestinationModel.States.Valid))
            {
                _soundService.ChoiceCorrect();

                RaisePropertyChanged(nameof(ConceptChoices));
                await Task.Delay(750);

                RaiseCompleted(ActivityAssignmentStatusIds.Successful);
                IsCompleted = true;
            }
            else
            {
                _soundService.ChoiceIncorrect();

                RaisePropertyChanged(nameof(ConceptChoices));
                await Task.Delay(2250);

                if (FirstTry)
                {
                    FirstTry = false;

                    foreach (var conceptChoice in ConceptChoices)
                    {
                        var destination = conceptChoice.Destination;

                        if (destination!.State == DestinationModel.States.Valid)
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
        Activity.Assignment.ActivityChoiceSelections.Add(new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = Activity.Assignment.Id,
            // Red herrings use an empty guid for the ID that gets saved
            ChoiceId = ConceptChoices.Any(x => x.Choice.Id == choiceId) ? choiceId : Guid.Empty,
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