namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ListenPartEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IListenPartService ListenPartService { get; set; } = null!;

    public ListenPartEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var assessmentId = Path!.Id("assessment");

        Model = new(Path, Id, IsNew, assessmentId, EventBus, Navigator, ListenPartService);
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

public class ListenPartEditModel : EditModel<ListenPart>,
    IHandle<ListenPartSaved>, IHandle<ListenPartRemoved>, IHandle<ListenPartsReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _assessmentId;
    private readonly INavigator _navigator;
    private readonly IListenPartService _listenPartService;

    public ListenPartEditModel(String? path, Guid? id, Boolean isNew, Guid? assessmentId,
        IEventBus eventBus,
        INavigator navigator,
        IListenPartService listenPartService) : base(isNew)
    {
        _path = path;
        _id = id;
        _assessmentId = assessmentId;
        _navigator = navigator;
        _listenPartService = listenPartService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ListenPartSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ListenPartRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(ListenPartsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _listenPartService.Fetch(new(new() { Id = _id }));

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

            var listenPart = new ListenPart
            {
                AssessmentId = _assessmentId,
                Title = "New Listen Part",
            };

            SetListenPart(listenPart);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _listenPartService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetListenPart(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _listenPartService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/listen-part-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _listenPartService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void SetListenPart(ListenPart listenPart)
    {
        Entity = listenPart;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}