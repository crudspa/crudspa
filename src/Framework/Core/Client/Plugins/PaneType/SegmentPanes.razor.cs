using Crudspa.Framework.Core.Shared.Contracts.Config.SegmentType;
using Crudspa.Framework.Core.Shared.Contracts.Ids;

namespace Crudspa.Framework.Core.Client.Plugins.PaneType;

public partial class SegmentPanes : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Boolean IsNew { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISegmentService SegmentService { get; set; } = null!;

    public SegmentPanesModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, EventBus, SegmentService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SegmentPanesModel : EditModel<Segment>,
    IHandle<SegmentSaved>,
    IHandle<SegmentMoved>,
    IHandle<PaneMoved>,
    IHandle<PanesReordered>,
    IHandle<PaneSaved>,
    IHandle<PaneRemoved>
{
    private readonly HashSet<String> _loadedPaneKeys = new(StringComparer.OrdinalIgnoreCase);
    private readonly Guid? _id;
    private readonly ISegmentService _segmentService;

    public SegmentPanesModel(Guid? id,
        IEventBus eventBus,
        ISegmentService segmentService) : base(false)
    {
        _id = id;
        _segmentService = segmentService;

        eventBus.Subscribe(this);
    }

    public TabbedPanesConfig TabbedPanesConfig
    {
        get;
        private set => SetProperty(ref field, value);
    } = new();

    public IEnumerable<Pane> OrderedPanes =>
        Entity?.Panes.OrderBy(x => x.Ordinal) ?? Enumerable.Empty<Pane>();

    public Pane? SinglePane => OrderedPanes.FirstOrDefault();

    public Boolean IsSinglePaneType =>
        Entity?.TypeId.HasValue == true &&
        Entity.TypeId.Value.Equals(SegmentTypeIds.SinglePage);

    public Boolean IsTabbedPanesType =>
        Entity?.TypeId.HasValue == true &&
        Entity.TypeId.Value.Equals(SegmentTypeIds.TabbedPages);

    public String ActivePaneKey
    {
        get;
        private set => SetProperty(ref field, value);
    } = String.Empty;

    public async Task Handle(SegmentSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public async Task Handle(SegmentMoved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public async Task Handle(PaneMoved payload)
    {
        if (payload.OldSegmentId.Equals(_id) || payload.NewSegmentId.Equals(_id))
            await Refresh();
    }

    public async Task Handle(PanesReordered payload)
    {
        if (payload.SegmentId.Equals(_id))
            await Refresh();
    }

    public async Task Handle(PaneSaved payload)
    {
        if (payload.SegmentId.Equals(_id))
            await Refresh();
    }

    public async Task Handle(PaneRemoved payload)
    {
        if (payload.SegmentId.Equals(_id))
            await Refresh();
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Fetching...", () => _segmentService.FetchStructure(new(new() { Id = _id })));

        if (!response.Ok)
            return;

        Entity = response.Value;
        TabbedPanesConfig = Entity.ConfigJson.FromJson<TabbedPanesConfig>() ?? new();

        var firstPaneKey = OrderedPanes.FirstOrDefault()?.Key ?? String.Empty;

        if (OrderedPanes.Any(x => x.Key.IsBasically(ActivePaneKey)) == false)
            ActivePaneKey = firstPaneKey;

        TrimLoadedPanes();
        EnsureActivePaneLoaded();

        RaisePropertyChanged(nameof(OrderedPanes));
        RaisePropertyChanged(nameof(SinglePane));
        RaisePropertyChanged(nameof(IsSinglePaneType));
        RaisePropertyChanged(nameof(IsTabbedPanesType));
    }

    public void SetActivePane(String? key)
    {
        if (key.HasNothing())
            return;

        if (OrderedPanes.Any(x => x.Key.IsBasically(key)) == false)
            return;

        ActivePaneKey = key!;
        EnsureActivePaneLoaded();
    }

    public Boolean IsPaneLoaded(String? key)
    {
        return key.HasSomething() && _loadedPaneKeys.Contains(key!);
    }

    private void EnsureActivePaneLoaded()
    {
        if (ActivePaneKey.HasSomething())
            _loadedPaneKeys.Add(ActivePaneKey);
    }

    private void TrimLoadedPanes()
    {
        var activeKeys = OrderedPanes
            .Select(x => x.Key)
            .Where(x => x.HasSomething())
            .Select(x => x!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _loadedPaneKeys.RemoveWhere(x => !activeKeys.Contains(x));
    }
}