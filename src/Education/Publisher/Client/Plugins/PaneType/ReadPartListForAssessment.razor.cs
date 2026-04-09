namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ReadPartListForAssessment : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IReadPartService ReadPartService { get; set; } = null!;

    public ReadPartListForAssessmentModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ReadPartService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ReadPartListForAssessmentModel : ListOrderablesModel<ReadPartModel>,
    IHandle<ReadPartAdded>, IHandle<ReadPartSaved>, IHandle<ReadPartRemoved>,
    IHandle<ReadParagraphAdded>, IHandle<ReadParagraphRemoved>,
    IHandle<ReadQuestionAdded>, IHandle<ReadQuestionRemoved>
{
    private readonly IReadPartService _readPartService;
    private readonly Guid? _assessmentId;

    public ReadPartListForAssessmentModel(IEventBus eventBus, IScrollService scrollService, IReadPartService readPartService, Guid? assessmentId)
        : base(scrollService)
    {
        _readPartService = readPartService;
        _assessmentId = assessmentId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ReadPartAdded payload) => await Replace(payload.Id, payload.AssessmentId);
    public async Task Handle(ReadPartSaved payload) => await Replace(payload.Id, payload.AssessmentId);
    public async Task Handle(ReadPartRemoved payload) => await Rid(payload.Id, payload.AssessmentId);

    public async Task Handle(ReadParagraphAdded payload)
    {
        if (payload.ReadPartId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.ReadPartId)))
            await Replace(payload.ReadPartId);
    }

    public async Task Handle(ReadParagraphRemoved payload)
    {
        if (payload.ReadPartId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.ReadPartId)))
            await Replace(payload.ReadPartId);
    }

    public async Task Handle(ReadQuestionAdded payload)
    {
        if (payload.ReadPartId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.ReadPartId)))
            await Replace(payload.ReadPartId);
    }

    public async Task Handle(ReadQuestionRemoved payload)
    {
        if (payload.ReadPartId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.ReadPartId)))
            await Replace(payload.ReadPartId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Assessment>(new() { Id = _assessmentId });
        var response = await WithWaiting("Fetching...", () => _readPartService.FetchForAssessment(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ReadPartModel(x)).ToList());
    }

    public override async Task<Response<ReadPartModel?>> Fetch(Guid? id)
    {
        var response = await _readPartService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ReadPartModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _readPartService.Remove(new(new()
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
        var orderables = Cards.Select(x => x.Entity.ReadPart).ToList();
        return await WithWaiting("Saving...", () => _readPartService.SaveOrder(new(orderables)));
    }
}

public class ReadPartModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleReadPartChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ReadPart));

    private ReadPart _readPart;

    public String? Name => ReadPart.Title;

    public ReadPartModel(ReadPart readPart)
    {
        _readPart = readPart;
        _readPart.PropertyChanged += HandleReadPartChanged;
    }

    public void Dispose()
    {
        _readPart.PropertyChanged -= HandleReadPartChanged;
    }

    public Guid? Id
    {
        get => _readPart.Id;
        set => _readPart.Id = value;
    }

    public Int32? Ordinal
    {
        get => _readPart.Ordinal;
        set => _readPart.Ordinal = value;
    }

    public ReadPart ReadPart
    {
        get => _readPart;
        set => SetProperty(ref _readPart, value);
    }
}