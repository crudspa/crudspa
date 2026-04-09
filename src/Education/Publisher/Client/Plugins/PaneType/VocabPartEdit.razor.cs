namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class VocabPartEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IVocabPartService VocabPartService { get; set; } = null!;

    public VocabPartEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var assessmentId = Path!.Id("assessment");

        Model = new(Path, Id, IsNew, assessmentId, EventBus, Navigator, VocabPartService);
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

public class VocabPartEditModel : EditModel<VocabPart>,
    IHandle<VocabPartSaved>, IHandle<VocabPartRemoved>, IHandle<VocabPartsReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _assessmentId;
    private readonly INavigator _navigator;
    private readonly IVocabPartService _vocabPartService;

    public VocabPartEditModel(String? path, Guid? id, Boolean isNew, Guid? assessmentId,
        IEventBus eventBus,
        INavigator navigator,
        IVocabPartService vocabPartService) : base(isNew)
    {
        _path = path;
        _id = id;
        _assessmentId = assessmentId;
        _navigator = navigator;
        _vocabPartService = vocabPartService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(VocabPartSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(VocabPartRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(VocabPartsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _vocabPartService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title);
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

            var vocabPart = new VocabPart
            {
                AssessmentId = _assessmentId,
                Title = "New Vocab Part",
            };

            SetVocabPart(vocabPart);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _vocabPartService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetVocabPart(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _vocabPartService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/vocab-part-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _vocabPartService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void SetVocabPart(VocabPart vocabPart)
    {
        Entity = vocabPart;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}