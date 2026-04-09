namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ListenQuestionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IListenQuestionService ListenQuestionService { get; set; } = null!;

    public ListenQuestionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var listenPartId = Path!.Id("listen-part");

        Model = new(Path, Id, IsNew, listenPartId, EventBus, Navigator, ListenQuestionService);
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

public class ListenQuestionEditModel : EditModel<ListenQuestion>,
    IHandle<ListenQuestionSaved>, IHandle<ListenQuestionRemoved>, IHandle<ListenQuestionsReordered>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(args.PropertyName);
    public BatchModel<ListenChoice> ListenChoicesModel { get; } = new();
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _listenPartId;
    private readonly INavigator _navigator;
    private readonly IListenQuestionService _listenQuestionService;

    public ListenQuestionEditModel(String? path, Guid? id, Boolean isNew, Guid? listenPartId,
        IEventBus eventBus,
        INavigator navigator,
        IListenQuestionService listenQuestionService) : base(isNew)
    {
        _path = path;
        _id = id;
        _listenPartId = listenPartId;
        _navigator = navigator;
        _listenQuestionService = listenQuestionService;

        ListenChoicesModel.PropertyChanged += HandleModelChanged;

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        ListenChoicesModel.PropertyChanged -= HandleModelChanged;

        base.Dispose();
    }

    public async Task Handle(ListenQuestionSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ListenQuestionRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(ListenQuestionsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _listenQuestionService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Name!);
        }
    }

    public List<Orderable> ListenQuestionCategoryNames
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
            FetchListenQuestionCategoryNames(),
            FetchReadQuestionTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetListenQuestion(new()
            {
                ListenPartId = _listenPartId,
                Text = String.Empty,
                IsPreview = false,
                PageBreakBefore = false,
                HasCorrectChoice = false,
                RequireTextInput = false,
                CategoryId = ListenQuestionCategoryNames.MinBy(x => x.Ordinal)?.Id,
                TypeId = ReadQuestionTypeNames.MinBy(x => x.Ordinal)?.Id,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _listenQuestionService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetListenQuestion(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _listenQuestionService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/listen-question-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _listenQuestionService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public void AddListenChoice()
    {
        ListenChoicesModel.Entities.Add(new()
        {
            Id = Guid.NewGuid(),
            ListenQuestionId = _id,
            IsCorrect = false,
            Ordinal = ListenChoicesModel.Entities.Count,
        });
    }

    public async Task FetchListenQuestionCategoryNames()
    {
        var response = await WithAlerts(() => _listenQuestionService.FetchListenQuestionCategoryNames(new()), false);
        if (response.Ok) ListenQuestionCategoryNames = response.Value.ToList();
    }

    public async Task FetchReadQuestionTypeNames()
    {
        var response = await WithAlerts(() => _listenQuestionService.FetchReadQuestionTypeNames(new()), false);
        if (response.Ok) ReadQuestionTypeNames = response.Value.ToList();
    }

    private void SetListenQuestion(ListenQuestion listenQuestion)
    {
        Entity = listenQuestion;
        ListenChoicesModel.Entities = listenQuestion.ListenChoices;
        _navigator.UpdateTitle(_path, Entity.Name!);
    }
}