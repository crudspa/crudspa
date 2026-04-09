namespace Crudspa.Education.Common.Client.Plugins.ActivityType;

public partial class FillBlanks : IActivityDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleCompleted(Object? sender, Guid status) => ActivityCompleted.InvokeAsync(status);

    [Parameter] public Activity? Activity { get; set; }
    [Parameter] public Guid? AssignmentBatchId { get; set; }
    [Parameter] public EventCallback<Guid> ActivityCompleted { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IActivityMediaPlayService ActivityMediaPlayService { get; set; } = null!;

    public FillBlanksModel Model { get; set; } = null!;

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

public class FillBlanksModel : Observable
{
    public event EventHandler<Guid>? Completed;

    private readonly IEventBus _eventBus;
    private readonly ISoundService _soundService;

    public Activity Activity { get; }
    public ObservableCollection<PieceModel> Options { get; set; } = null!;
    public ObservableCollection<ActivityChoice> Choices { get; set; }
    public ObservableCollection<LineModel> Lines { get; set; }

    public FillBlanksModel(IEventBus eventBus, ISoundService soundService, Activity activity)
    {
        _eventBus = eventBus;
        _soundService = soundService;

        Activity = activity;

        Choices = activity.ActivityChoices;

        InitializeOptions();

        Lines = ToLineModels(activity.ContextText!);
    }

    public Boolean HasInputs
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void InitializeOptions()
    {
        Options = Choices.Shuffle().Select(x => new PieceModel(x)).ToObservable();
    }

    public void Reset()
    {
        InitializeOptions();

        foreach (var line in Lines)
        {
            line.IsValidating = false;

            if (line.IsInput == true || line.IsTallInput == true)
                line.EntryText = String.Empty;

            else if (line.IsChoice == true)
            {
                line.DestinationModel!.PlacedPiece = null;
                line.DestinationModel.State = DestinationModel.States.Empty;
            }
        }

        Activity.Assignment.ActivityChoiceSelections = [];
    }

    public Task SelectOption(Guid id)
    {
        _soundService.ChoiceSelected();

        foreach (var option in Options)
            option.State = option.Id.Equals(id) && option.State != PieceModel.States.Selected
                ? PieceModel.States.Selected
                : PieceModel.States.Default;

        RaisePropertyChanged(nameof(Options));

        return Task.CompletedTask;
    }

    public async Task SelectDestination(Guid modelId)
    {
        foreach (var line in Lines.Where(x => x.IsChoice == true))
        {
            if (line.DestinationModel!.ModelId.Equals(modelId))
            {
                var destination = line.DestinationModel;

                if (destination is null
                    || destination.State == DestinationModel.States.Valid
                    || destination.State == DestinationModel.States.Invalid)
                    return;

                if (destination.State == DestinationModel.States.Filled)
                {
                    MovePieceBack(destination);
                    return;
                }

                var selectedOption = Options.FirstOrDefault(x => x.State == PieceModel.States.Selected);
                if (selectedOption is null) return;

                destination.PlacedPiece = selectedOption;
                destination.State = DestinationModel.States.Filled;

                Options.Remove(selectedOption);

                RecordSelection(selectedOption.Id!.Value);
            }
        }

        var allDestinationsFilled = Lines.Where(x => x.IsChoice == true)
            .All(x => x.DestinationModel!.State != DestinationModel.States.Empty);

        if (!HasInputs && allDestinationsFilled)
        {
            await EvaluateProgress();
        }
    }

    public void MovePieceBack(DestinationModel destination)
    {
        destination.PlacedPiece!.State = PieceModel.States.Default;

        Options.Add(destination.PlacedPiece);

        destination.PlacedPiece = null;
        destination.State = DestinationModel.States.Empty;
    }

    public async Task EvaluateProgress()
    {
        await _eventBus.Publish(new SilenceRequested());

        var valid = Lines.All(x => !x.NeedsAttention());

        if (!valid) return;

        var choiceLines = Lines.Where(x => x.IsChoice == true).ToList();
        var destinations = choiceLines.Select(x => x.DestinationModel).ToList();

        foreach (var destination in destinations)
        {
            destination!.State = destination.TargetIds!.Any(x => x.Equals(destination.PlacedPiece!.Id!.Value))
                ? DestinationModel.States.Valid
                : DestinationModel.States.Invalid;
        }

        if (destinations.All(x => x!.State.Equals(DestinationModel.States.Valid)))
        {
            _soundService.ChoiceCorrect();
            AddActivityTexts();

            RaisePropertyChanged(nameof(Options));

            await Task.Delay(750);
            RaiseCompleted(ActivityAssignmentStatusIds.Successful);
        }
        else
        {
            _soundService.ChoiceIncorrect();
            AddActivityTexts();

            RaisePropertyChanged(nameof(Options));

            await Task.Delay(2250);
            RaiseCompleted(ActivityAssignmentStatusIds.Failed);
        }
    }

    private void AddActivityTexts()
    {
        var lineModels = Lines.Where(x => x.IsInput == true || x.IsTallInput == true).ToList();

        foreach (var lineModel in lineModels)
        {
            Activity.Assignment.TextEntries.Add(new()
            {
                Id = Guid.NewGuid(),
                AssignmentId = Activity.Assignment.Id,
                Made = DateTime.Now,
                Text = lineModel.EntryText,
                Ordinal = lineModels.IndexOf(lineModel),
            });
        }
    }

    private void RecordSelection(Guid choiceId)
    {
        Activity.Assignment.ActivityChoiceSelections.Add(new()
        {
            Id = Guid.NewGuid(),
            AssignmentId = Activity.Assignment.Id,
            ChoiceId = choiceId,
            Made = DateTime.Now,
        });
    }

    private void RaiseCompleted(Guid status)
    {
        var raiseEvent = Completed;
        raiseEvent?.Invoke(this, status);
    }

    private ObservableCollection<LineModel> ToLineModels(String contextText)
    {
        var models = new List<LineModel>();
        var input = contextText;
        HasInputs = false;

        while (!String.IsNullOrWhiteSpace(input))
        {
            var openBracketIndex = input.IndexOf("[", StringComparison.Ordinal);

            // No open brackets ahead, make the remaining text a label and bail
            if (openBracketIndex < 0)
            {
                if (!String.IsNullOrWhiteSpace(input))
                {
                    var lineModel = new LineModel
                    {
                        IsLabel = true,
                        LabelText = input.Trim(),
                    };
                    models.Add(lineModel);
                }

                break;
            }

            // Find the corresponding close bracket
            var closeBracketIndex = input.IndexOf("]", StringComparison.Ordinal);

            // Brackets ahead, everything before this is a label
            if (openBracketIndex > 0 && closeBracketIndex > openBracketIndex)
            {
                var lineModel = new LineModel
                {
                    IsLabel = true,
                    LabelText = input.Substring(0, openBracketIndex).Trim(),
                };
                models.Add(lineModel);
            }

            // No corresponding close bracket, make the remaining text a label and bail
            if (closeBracketIndex < 1)
            {
                if (!String.IsNullOrWhiteSpace(input))
                {
                    var lineModel = new LineModel
                    {
                        IsLabel = true,
                        LabelText = input.Trim(),
                    };
                    models.Add(lineModel);
                }

                break;
            }

            // Parse the token name and add the proper type of LineModel
            var token = input.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);

            if (token.ToLower() == "input")
            {
                var lineModel = new LineModel { IsInput = true };
                HasInputs = true;
                models.Add(lineModel);
            }
            else if (token.ToLower() == "tallinput")
            {
                var lineModel = new LineModel { IsTallInput = true };
                HasInputs = true;
                models.Add(lineModel);
            }
            else if (token.StartsWith("choice"))
            {
                // Multiple valid choices can be specified, e.g. [Choice1|2]
                var numberPart = token.Substring("choice".Length);
                var ordinalStrings = numberPart.Split("|");
                var validIndexes = new List<Int32>();

                foreach (var ordinalString in ordinalStrings)
                    validIndexes.Add(Int32.Parse(ordinalString) - 1);

                if (validIndexes.Count > 0)
                {
                    var targetIds = new List<Guid>();

                    foreach (var index in validIndexes)
                        if (Activity.ActivityChoices.Count > index)
                            targetIds.Add(Activity.ActivityChoices[index].Id!.Value);

                    var lineModel = new LineModel
                    {
                        IsChoice = true,
                        DestinationModel = new(null, targetIds),
                    };
                    models.Add(lineModel);
                }
            }

            // Move on, starting after the close bracket
            input = input.Substring(closeBracketIndex + 1);
        }

        return models.ToObservable();
    }
}