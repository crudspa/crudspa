namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ListenPartListForAssessment : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IListenPartService ListenPartService { get; set; } = null!;

    public ListenPartListForAssessmentModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ListenPartService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ListenPartListForAssessmentModel : ListOrderablesModel<ListenPartModel>,
    IHandle<ListenPartAdded>, IHandle<ListenPartSaved>, IHandle<ListenPartRemoved>,
    IHandle<ListenQuestionAdded>, IHandle<ListenQuestionRemoved>
{
    private readonly IListenPartService _listenPartService;
    private readonly Guid? _assessmentId;

    public ListenPartListForAssessmentModel(IEventBus eventBus, IScrollService scrollService, IListenPartService listenPartService, Guid? assessmentId)
        : base(scrollService)
    {
        _listenPartService = listenPartService;
        _assessmentId = assessmentId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ListenPartAdded payload) => await Replace(payload.Id, payload.AssessmentId);
    public async Task Handle(ListenPartSaved payload) => await Replace(payload.Id, payload.AssessmentId);
    public async Task Handle(ListenPartRemoved payload) => await Rid(payload.Id, payload.AssessmentId);

    public async Task Handle(ListenQuestionAdded payload)
    {
        if (payload.ListenPartId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.ListenPartId)))
            await Replace(payload.ListenPartId);
    }

    public async Task Handle(ListenQuestionRemoved payload)
    {
        if (payload.ListenPartId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.ListenPartId)))
            await Replace(payload.ListenPartId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Assessment>(new() { Id = _assessmentId });
        var response = await WithWaiting("Fetching...", () => _listenPartService.FetchForAssessment(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ListenPartModel(x)).ToList());
    }

    public override async Task<Response<ListenPartModel?>> Fetch(Guid? id)
    {
        var response = await _listenPartService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ListenPartModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _listenPartService.Remove(new(new()
        {
            Id = id,
            AssessmentId = _assessmentId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_assessmentId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.ListenPart).ToList();
        return await WithWaiting("Saving...", () => _listenPartService.SaveOrder(new(orderables)));
    }
}

public class ListenPartModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleListenPartChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ListenPart));

    private ListenPart _listenPart;

    public String? Name => ListenPart.Title;

    public ListenPartModel(ListenPart listenPart)
    {
        _listenPart = listenPart;
        _listenPart.PropertyChanged += HandleListenPartChanged;
    }

    public void Dispose()
    {
        _listenPart.PropertyChanged -= HandleListenPartChanged;
    }

    public Guid? Id
    {
        get => _listenPart.Id;
        set => _listenPart.Id = value;
    }

    public Int32? Ordinal
    {
        get => _listenPart.Ordinal;
        set => _listenPart.Ordinal = value;
    }

    public ListenPart ListenPart
    {
        get => _listenPart;
        set => SetProperty(ref _listenPart, value);
    }
}