namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ReadQuestionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IReadQuestionService ReadQuestionService { get; set; } = null!;

    public ReadQuestionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var readPartId = Path!.Id("read-part");

        Model = new(Path, Id, IsNew, readPartId, EventBus, Navigator, ReadQuestionService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class ReadQuestionEditModel : EditModel<ReadQuestion>,
    IHandle<ReadQuestionSaved>, IHandle<ReadQuestionRemoved>, IHandle<ReadQuestionsReordered>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(args.PropertyName);
    public BatchModel<ReadChoice> ReadChoicesModel { get; } = new();
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _readPartId;
    private readonly INavigator _navigator;
    private readonly IReadQuestionService _readQuestionService;

    public ReadQuestionEditModel(String? path, Guid? id, Boolean isNew, Guid? readPartId,
        IEventBus eventBus,
        INavigator navigator,
        IReadQuestionService readQuestionService) : base(isNew)
    {
        _path = path;
        _id = id;
        _readPartId = readPartId;
        _navigator = navigator;
        _readQuestionService = readQuestionService;

        ReadChoicesModel.PropertyChanged += HandleModelChanged;

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        ReadChoicesModel.PropertyChanged -= HandleModelChanged;

        base.Dispose();
    }

    public async Task Handle(ReadQuestionSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ReadQuestionRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(ReadQuestionsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _readQuestionService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Name);
        }
    }

    public List<Orderable> ReadQuestionCategoryNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ReadQuestionTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchReadQuestionCategoryNames(),
            FetchReadQuestionTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetReadQuestion(new()
            {
                ReadPartId = _readPartId,
                Text = String.Empty,
                IsPreview = false,
                PageBreakBefore = false,
                HasCorrectChoice = false,
                RequireTextInput = false,
                CategoryId = ReadQuestionCategoryNames.MinBy(x => x.Ordinal)?.Id,
                TypeId = ReadQuestionTypeNames.MinBy(x => x.Ordinal)?.Id,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _readQuestionService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetReadQuestion(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _readQuestionService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/read-question-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _readQuestionService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public void AddReadChoice()
    {
        ReadChoicesModel.Entities.Add(new()
        {
            Id = Guid.NewGuid(),
            ReadQuestionId = _id,
            IsCorrect = false,
            Ordinal = ReadChoicesModel.Entities.Count,
        });
    }

    public async Task FetchReadQuestionCategoryNames()
    {
        var response = await WithAlerts(() => _readQuestionService.FetchReadQuestionCategoryNames(new()), false);
        if (response.Ok) ReadQuestionCategoryNames = response.Value.ToList();
    }

    public async Task FetchReadQuestionTypeNames()
    {
        var response = await WithAlerts(() => _readQuestionService.FetchReadQuestionTypeNames(new()), false);
        if (response.Ok) ReadQuestionTypeNames = response.Value.ToList();
    }

    private void SetReadQuestion(ReadQuestion readQuestion)
    {
        Entity = readQuestion;
        ReadChoicesModel.Entities = readQuestion.ReadChoices;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}