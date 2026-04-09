namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class VocabQuestionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IVocabQuestionService VocabQuestionService { get; set; } = null!;

    public VocabQuestionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var vocabPartId = Path!.Id("vocab-part");

        Model = new(Path, Id, IsNew, vocabPartId, EventBus, Navigator, VocabQuestionService);
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

public class VocabQuestionEditModel : EditModel<VocabQuestion>,
    IHandle<VocabQuestionSaved>, IHandle<VocabQuestionRemoved>, IHandle<VocabQuestionsReordered>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(args.PropertyName);
    public BatchModel<VocabChoice> VocabChoicesModel { get; } = new();
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _vocabPartId;
    private readonly INavigator _navigator;
    private readonly IVocabQuestionService _vocabQuestionService;

    public VocabQuestionEditModel(String? path, Guid? id, Boolean isNew, Guid? vocabPartId,
        IEventBus eventBus,
        INavigator navigator,
        IVocabQuestionService vocabQuestionService) : base(isNew)
    {
        _path = path;
        _id = id;
        _vocabPartId = vocabPartId;
        _navigator = navigator;
        _vocabQuestionService = vocabQuestionService;

        VocabChoicesModel.PropertyChanged += HandleModelChanged;

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        VocabChoicesModel.PropertyChanged -= HandleModelChanged;

        base.Dispose();
    }

    public async Task Handle(VocabQuestionSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(VocabQuestionRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(VocabQuestionsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _vocabQuestionService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Name);
        }
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetVocabQuestion(new()
            {
                VocabPartId = _vocabPartId,
                Word = "word",
                IsPreview = false,
                PageBreakBefore = false,
            });

            for (var i = 0; i < 4; i++)
                AddVocabChoice();
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _vocabQuestionService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetVocabQuestion(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _vocabQuestionService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/vocab-question-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _vocabQuestionService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public void AddVocabChoice()
    {
        VocabChoicesModel.Entities.Add(new()
        {
            Id = Guid.NewGuid(),
            VocabQuestionId = _id,
            Word = String.Empty,
            IsCorrect = false,
            Ordinal = VocabChoicesModel.Entities.Count,
        });
    }

    private void SetVocabQuestion(VocabQuestion vocabQuestion)
    {
        Entity = vocabQuestion;
        VocabChoicesModel.Entities = vocabQuestion.VocabChoices;
        _navigator.UpdateTitle(_path, Entity.Name!);
    }
}