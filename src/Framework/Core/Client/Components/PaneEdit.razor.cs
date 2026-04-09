using Crudspa.Framework.Core.Client.Plugins;

namespace Crudspa.Framework.Core.Client.Components;

public partial class PaneEdit : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPaneService PaneService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISegmentService SegmentService { get; set; } = null!;

    public PaneEditModel Model { get; set; } = null!;
    public PaneMoveModel MoveModel { get; set; } = null!;
    public PaneDesignPlugin? DesignComponent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, PaneService);
        Model.PropertyChanged += HandleModelChanged;

        MoveModel = new(ScrollService, SegmentService, PaneService);
        MoveModel.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        MoveModel.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
        MoveModel.Dispose();
    }

    private async Task HandleSaveClicked()
    {
        if (DesignComponent?.Instance is IPaneDesign design)
        {
            var saved = await design.PrepareForSave();

            if (!saved)
                return;

            Model.Entity!.ConfigJson = design.GetConfigJson();
        }

        await Model.Save();
    }

    private async Task HandleConfigUpdated()
    {
        if (DesignComponent?.Instance is IPaneDesign design)
            Model.Entity!.ConfigJson = design.GetConfigJson();

        await Model.Save();
    }

    private async Task HandleMoveClicked()
    {
        var oldSegmentId = Model.Entity?.SegmentId;

        await MoveModel.MovePane();

        var response = await PaneService.Fetch(new(new() { Id = Model.Entity?.Id }));

        if (!response.Ok)
            return;

        var updated = response.Value;

        if (oldSegmentId.HasValue && updated.SegmentId.HasValue && !oldSegmentId.Value.Equals(updated.SegmentId.Value))
        {
            var newPath = Path!.Replace($"/segment-{oldSegmentId:D}", $"/segment-{updated.SegmentId:D}");
            var separator = newPath.Contains('?') ? "&" : "?";
            Navigator.GoTo(newPath + separator + "pane=content");
            return;
        }

        await Model.Refresh();
    }
}

public class PaneEditModel : EditModel<Pane>, IHandle<PaneSaved>, IHandle<PaneMoved>, IHandle<PaneRemoved>, IHandle<PanesReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IPaneService _paneService;

    public PaneEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IPaneService paneService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _paneService = paneService;

        eventBus.Subscribe(this);
    }

    public Boolean HasDesigner => Entity?.TypeEditorView.HasSomething() == true;

    public async Task Handle(PaneSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public async Task Handle(PaneMoved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(PaneRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(PanesReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _paneService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Name!);
        }
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _paneService.Fetch(new(new() { Id = _id })));

        if (response.Ok)
            Entity = response.Value;
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _paneService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }
}