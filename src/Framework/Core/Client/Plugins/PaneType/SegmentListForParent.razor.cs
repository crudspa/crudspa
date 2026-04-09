namespace Crudspa.Framework.Core.Client.Plugins.PaneType;

public partial class SegmentListForParent : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISegmentService SegmentService { get; set; } = null!;

    public SegmentListForParentModel Model { get; set; } = null!;
    public SegmentMoveModel MoveModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SegmentService, Id);
        Model.PropertyChanged += HandleModelChanged;

        MoveModel = new(ScrollService, SegmentService);
        MoveModel.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        MoveModel.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SegmentListForParentModel : ListOrderablesModel<SegmentModel>,
    IHandle<SegmentAdded>, IHandle<SegmentSaved>, IHandle<SegmentRemoved>, IHandle<SegmentsReordered>, IHandle<SegmentMoved>,
    IHandle<LicenseAdded>, IHandle<LicenseSaved>, IHandle<LicenseRemoved>
{
    private readonly ISegmentService _segmentService;
    private readonly Guid? _parentId;

    public SegmentListForParentModel(IEventBus eventBus, IScrollService scrollService, ISegmentService segmentService, Guid? parentId)
        : base(scrollService)
    {
        _segmentService = segmentService;

        _parentId = parentId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SegmentAdded payload) => await Replace(payload.Id, payload.ParentId);

    public async Task Handle(SegmentSaved payload) => await Refresh(false);

    public async Task Handle(SegmentRemoved payload) => await Rid(payload.Id, payload.ParentId);

    public async Task Handle(SegmentsReordered payload) => await Refresh();

    public async Task Handle(SegmentMoved payload)
    {
        if (payload.OldParentId.Equals(_parentId) || payload.NewParentId.Equals(_parentId))
            await Refresh();
    }

    public async Task Handle(LicenseAdded payload) => await Refresh();

    public async Task Handle(LicenseSaved payload) => await Refresh();

    public async Task Handle(LicenseRemoved payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Segment>(new() { Id = _parentId });
        var response = await WithWaiting("Fetching...", () => _segmentService.FetchForParent(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new SegmentModel(x)).ToList());
    }

    public override async Task<Response<SegmentModel?>> Fetch(Guid? id)
    {
        var response = await _segmentService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new SegmentModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _segmentService.Remove(new(new()
        {
            Id = id,
            ParentId = _parentId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_parentId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Segment).ToList();
        return await WithWaiting("Saving...", () => _segmentService.SaveOrder(new(orderables)));
    }
}