using Crudspa.Framework.Core.Shared.Contracts.Ids;

namespace Crudspa.Framework.Core.Client.Plugins.PaneType;

public partial class SegmentEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISegmentService SegmentService { get; set; } = null!;
    [Inject] public IPaneService PaneService { get; set; } = null!;

    public SegmentEditModel Model { get; set; } = null!;
    public SegmentDesignPlugin? DesignComponent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal");
        var parentId = Path.Id("segment", 1);

        Model = new(Path, Id, IsNew, portalId, parentId, EventBus, Navigator, SegmentService, PaneService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleSaveClicked()
    {
        if (DesignComponent?.Instance is ISegmentDesign design)
            Model.Entity!.ConfigJson = design.GetConfigJson();

        await Model.Save();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class SegmentEditModel : EditModel<Segment>,
    IHandle<SegmentSaved>, IHandle<SegmentMoved>, IHandle<SegmentRemoved>, IHandle<SegmentsReordered>,
    IHandle<PaneMoved>, IHandle<PaneSaved>, IHandle<PaneRemoved>, IHandle<PanesReordered>,
    IHandle<LicenseAdded>, IHandle<LicenseSaved>, IHandle<LicenseRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _portalId;
    private readonly Guid? _parentId;
    private readonly INavigator _navigator;
    private readonly ISegmentService _segmentService;
    private readonly IPaneService _paneService;
    private readonly Dictionary<Guid, Guid?> _originalPaneTypes = [];
    private List<PaneTypeFull> _paneTypes = [];

    public SegmentEditModel(String? path, Guid? id, Boolean isNew,
        Guid? portalId, Guid? parentId,
        IEventBus eventBus,
        INavigator navigator,
        ISegmentService segmentService,
        IPaneService paneService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _parentId = parentId;
        _navigator = navigator;
        _segmentService = segmentService;
        _paneService = paneService;

        eventBus.Subscribe(this);
    }

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

    public Task Handle(SegmentRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(SegmentsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _segmentService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Name!);
        }
    }

    public async Task Handle(PaneMoved payload)
    {
        if (payload.OldSegmentId.Equals(_id) || payload.NewSegmentId.Equals(_id))
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

    public async Task Handle(PanesReordered payload)
    {
        if (payload.SegmentId.Equals(_id))
            await Refresh();
    }

    public async Task Handle(LicenseAdded payload) => await FetchLicenseNames();

    public async Task Handle(LicenseSaved payload) => await FetchLicenseNames();

    public async Task Handle(LicenseRemoved payload) => await FetchLicenseNames();

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<IconFull> Icons
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> LicenseNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Named> PermissionNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<SegmentTypeFull> SegmentTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<PaneTypeFull> PaneTypes
    {
        get => _paneTypes;
        set => SetProperty(ref _paneTypes, value);
    }

    public SegmentTypeFull? SelectedSegmentType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public BatchModel<Pane> PanesModel { get; } = new();
    public Pane? SinglePane => Entity?.Panes.OrderBy(x => x.Ordinal).FirstOrDefault();

    public Boolean IsSinglePaneType =>
        Entity?.TypeId.HasValue == true &&
        Entity.TypeId.Value.Equals(SegmentTypeIds.SinglePage);

    public Boolean CanAddPane =>
        Entity is not null && (!IsSinglePaneType || Entity.Panes.Count == 0);

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchIcons(),
            FetchLicenseNames(),
            FetchPermissionNames(),
            FetchSegmentTypes(),
            FetchPaneTypes());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var newSegment = new Segment
            {
                Id = _id ?? Guid.NewGuid(),
                PortalId = _portalId,
                ParentId = _parentId,
                Key = String.Empty,
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                Title = "New Segment",
                Fixed = true,
                RequiresId = false,
                Recursive = false,
                TypeId = SegmentTypes.MinBy(x => x.Ordinal)?.Id,
                AllLicenses = true,
            };

            foreach (var license in LicenseNames)
            {
                newSegment.Licenses.Add(new()
                {
                    Id = license.Id,
                    Name = license.Name,
                    Selected = false,
                });
            }

            SetSegment(newSegment);
            return;
        }

        ReadOnly = true;

        var segmentTask = WithAlerts(() => _segmentService.Fetch(new(new() { Id = _id })), false);
        var structureTask = WithAlerts(() => _segmentService.FetchStructure(new(new() { Id = _id })), false);

        await WithMany("Fetching...", segmentTask, structureTask);

        var segmentResponse = await segmentTask;
        var structureResponse = await structureTask;

        if (!segmentResponse.Ok || !structureResponse.Ok)
            return;

        if (segmentResponse.Value is null || structureResponse.Value is null)
            return;

        var fetchedSegment = segmentResponse.Value;
        var structure = structureResponse.Value;

        fetchedSegment.TypeId = structure.TypeId;
        fetchedSegment.TypeName = structure.TypeName;
        fetchedSegment.TypeDisplayView = structure.TypeDisplayView;
        fetchedSegment.TypeEditorView = structure.TypeEditorView;
        fetchedSegment.ConfigJson = structure.ConfigJson;
        fetchedSegment.Panes = structure.Panes;

        SetSegment(fetchedSegment);
    }

    public async Task Save()
    {
        if (Entity is null)
            return;

        Alerts.Clear();

        NormalizePanes();

        if (!IsValid(Entity) || !ArePanesValid())
            return;

        if (IsNew)
        {
            var addResponse = await WithWaiting("Adding...", () => _segmentService.Add(new(Entity)));

            if (!addResponse.Ok || addResponse.Value?.Id is null)
                return;

            Entity.Id = addResponse.Value.Id;
            Entity.PortalId = addResponse.Value.PortalId;
            Entity.ParentId = addResponse.Value.ParentId;

            _navigator.GoTo($"{_path.Parent()}/segment-{Entity.Id:D}");
            _navigator.Close(_path);

            return;
        }

        ResetConfigs();

        var response = await WithWaiting("Saving...", () => _segmentService.Save(new(Entity)));

        if (!response.Ok)
            return;

        ReadOnly = true;
        SnapshotPaneTypes();
    }

    public void AddPane()
    {
        if (!CanAddPane || Entity is null)
            return;

        var ordinal = Entity.Panes.Count;

        Entity.Panes.Add(NewPane(ordinal));
        PanesModel.UpdateSort();
        RaisePropertyChanged(nameof(CanAddPane));
        RaisePropertyChanged(nameof(SinglePane));
    }

    public void SetSegmentType(Guid? segmentTypeId)
    {
        if (Entity is null)
            return;

        var wasSinglePaneType = IsSinglePaneType;

        Entity.TypeId = segmentTypeId;
        SetSelectedType();
        EnsurePaneStructure();

        if (!wasSinglePaneType && IsSinglePaneType)
            EnsureSinglePaneDefaults(overwrite: true);

        RaisePropertyChanged(nameof(IsSinglePaneType));
        RaisePropertyChanged(nameof(CanAddPane));
        RaisePropertyChanged(nameof(SinglePane));
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _segmentService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchIcons()
    {
        var response = await WithAlerts(() => _segmentService.FetchIcons(new()), false);
        if (response.Ok) Icons = response.Value.ToList();
    }

    public async Task FetchLicenseNames()
    {
        var response = await WithAlerts(() => _segmentService.FetchLicenseNames(new()), false);
        if (response.Ok) LicenseNames = response.Value.ToObservable();
    }

    public async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _segmentService.FetchPermissionNames(new(new() { Id = _portalId })), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    public async Task FetchSegmentTypes()
    {
        var response = await WithAlerts(() => _segmentService.FetchSegmentTypes(new(new() { Id = _portalId })), false);
        if (response.Ok) SegmentTypes = response.Value.ToList();
    }

    public async Task FetchPaneTypes()
    {
        var response = await WithAlerts(() => _paneService.FetchPaneTypes(new(new() { Id = _portalId })), false);
        if (response.Ok) PaneTypes = response.Value.ToList();
    }

    private Boolean ArePanesValid()
    {
        if (Entity is null)
            return false;

        var panesToValidate = IsSinglePaneType
            ? Entity.Panes.OrderBy(x => x.Ordinal).Take(1).ToList()
            : Entity.Panes.OrderBy(x => x.Ordinal).ToList();

        if (panesToValidate.Count == 0)
        {
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors =
                [
                    new() { Message = "At least one pane is required." },
                ],
            });

            return false;
        }

        var errors = panesToValidate.SelectMany(pane =>
        {
            var paneErrors = pane.Validate();

            var label = pane.Title.HasSomething() ? pane.Title : pane.Key.HasSomething() ? pane.Key : "Pane";

            foreach (var error in paneErrors)
                error.Message = $"{label}: {error.Message}";

            return paneErrors;
        });

        if (!errors.HasItems())
            return true;

        Alerts.Add(new()
        {
            Type = Alert.AlertType.Error,
            Errors = errors.ToList(),
        });

        return false;
    }

    private Pane NewPane(Int32 ordinal)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            SegmentId = Entity?.Id,
            Ordinal = ordinal,
            Key = String.Empty,
            Title = String.Empty,
        };
    }

    private void SetSelectedType()
    {
        if (Entity is not null && SegmentTypes.HasItems())
            SelectedSegmentType = SegmentTypes.FirstOrDefault(x => x.Id.Equals(Entity.TypeId));
    }

    private void SetSegment(Segment segment)
    {
        Entity = segment;

        EnsurePaneStructure();
        SnapshotPaneTypes();
        SetSelectedType();

        if (!ReferenceEquals(PanesModel.Entities, Entity.Panes))
            PanesModel.Entities = Entity.Panes;

        PanesModel.UpdateSort();
        RaisePropertyChanged(nameof(IsSinglePaneType));
        RaisePropertyChanged(nameof(CanAddPane));
        RaisePropertyChanged(nameof(SinglePane));

        _navigator.UpdateTitle(_path, Entity.Name!);
    }

    private void EnsurePaneStructure()
    {
        if (Entity is null)
            return;

        if (Entity.Panes.Count == 0)
            Entity.Panes.Add(NewPane(0));

        Entity.Panes.EnsureOrder();

        foreach (var pane in Entity.Panes)
            pane.SegmentId ??= Entity.Id;

        EnsureSinglePaneDefaults();
    }

    private void NormalizePanes()
    {
        if (Entity is null)
            return;

        Entity.Panes.EnsureOrder();

        foreach (var pane in Entity.Panes)
            pane.SegmentId = Entity.Id;

        EnsureSinglePaneDefaults();
    }

    private void EnsureSinglePaneDefaults(Boolean overwrite = false)
    {
        if (Entity is null || !IsSinglePaneType)
            return;

        var pane = Entity.Panes
            .OrderBy(x => x.Ordinal)
            .FirstOrDefault();

        if (pane is null)
            return;

        if (!overwrite && pane.Key.HasSomething() && pane.Title.HasSomething())
            return;

        var (key, title) = CreateSinglePaneDefaults(pane);

        pane.Key = key;
        pane.Title = title;
    }

    private (String key, String title) CreateSinglePaneDefaults(Pane targetPane)
    {
        var usedKeys = Entity?.Panes
            .Where(x => !ReferenceEquals(x, targetPane) && x.Key.HasSomething())
            .Select(x => x.Key!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        var usedTitles = Entity?.Panes
            .Where(x => !ReferenceEquals(x, targetPane) && x.Title.HasSomething())
            .Select(x => x.Title!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        var index = 1;

        while (true)
        {
            var key = index == 1 ? "detail" : $"detail{index}";
            var title = index == 1 ? "Detail" : $"Detail {index}";

            if (!usedKeys.Contains(key) && !usedTitles.Contains(title))
                return (key, title);

            index++;
        }
    }

    private void SnapshotPaneTypes()
    {
        _originalPaneTypes.Clear();

        if (Entity is null)
            return;

        foreach (var pane in Entity.Panes.Where(x => x.Id.HasValue))
            _originalPaneTypes[pane.Id!.Value] = pane.TypeId;
    }

    private void ResetConfigs()
    {
        if (Entity is null)
            return;

        foreach (var pane in Entity.Panes)
        {
            if (!pane.Id.HasValue)
                continue;

            if (!_originalPaneTypes.TryGetValue(pane.Id.Value, out var originalTypeId))
                continue;

            if (!originalTypeId.Equals(pane.TypeId))
                pane.ConfigJson = null;
        }
    }
}