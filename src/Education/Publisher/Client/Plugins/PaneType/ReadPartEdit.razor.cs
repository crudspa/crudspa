namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ReadPartEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IReadPartService ReadPartService { get; set; } = null!;

    public ReadPartEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var assessmentId = Path!.Id("assessment");

        Model = new(Path, Id, IsNew, assessmentId, EventBus, Navigator, ReadPartService);
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

public class ReadPartEditModel : EditModel<ReadPart>,
    IHandle<ReadPartSaved>, IHandle<ReadPartRemoved>, IHandle<ReadPartsReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _assessmentId;
    private readonly INavigator _navigator;
    private readonly IReadPartService _readPartService;

    public ReadPartEditModel(String? path, Guid? id, Boolean isNew, Guid? assessmentId,
        IEventBus eventBus,
        INavigator navigator,
        IReadPartService readPartService) : base(isNew)
    {
        _path = path;
        _id = id;
        _assessmentId = assessmentId;
        _navigator = navigator;
        _readPartService = readPartService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ReadPartSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ReadPartRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(ReadPartsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _readPartService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title!);
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

            SetReadPart(new()
            {
                AssessmentId = _assessmentId,
                Title = "New Read Part",
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _readPartService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetReadPart(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _readPartService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/read-part-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _readPartService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void SetReadPart(ReadPart readPart)
    {
        Entity = readPart;
        _navigator.UpdateTitle(_path, Entity.Title!);
    }
}