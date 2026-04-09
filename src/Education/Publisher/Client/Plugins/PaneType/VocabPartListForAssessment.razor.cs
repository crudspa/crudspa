namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class VocabPartListForAssessment : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IVocabPartService VocabPartService { get; set; } = null!;

    public VocabPartListForAssessmentModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, VocabPartService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class VocabPartListForAssessmentModel : ListOrderablesModel<VocabPartModel>,
    IHandle<VocabPartAdded>, IHandle<VocabPartSaved>, IHandle<VocabPartRemoved>, IHandle<VocabPartsReordered>,
    IHandle<VocabQuestionAdded>, IHandle<VocabQuestionRemoved>
{
    private readonly IVocabPartService _vocabPartService;
    private readonly Guid? _assessmentId;

    public VocabPartListForAssessmentModel(IEventBus eventBus, IScrollService scrollService, IVocabPartService vocabPartService, Guid? assessmentId)
        : base(scrollService)
    {
        _vocabPartService = vocabPartService;

        _assessmentId = assessmentId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(VocabPartAdded payload) => await Replace(payload.Id, payload.AssessmentId);

    public async Task Handle(VocabPartSaved payload) => await Replace(payload.Id, payload.AssessmentId);

    public async Task Handle(VocabPartRemoved payload) => await Rid(payload.Id, payload.AssessmentId);

    public async Task Handle(VocabPartsReordered payload) => await Refresh();

    public async Task Handle(VocabQuestionAdded payload)
    {
        if (payload.VocabPartId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.VocabPartId)))
            await Replace(payload.VocabPartId);
    }

    public async Task Handle(VocabQuestionRemoved payload)
    {
        if (payload.VocabPartId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.VocabPartId)))
            await Replace(payload.VocabPartId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Assessment>(new() { Id = _assessmentId });
        var response = await WithWaiting("Fetching...", () => _vocabPartService.FetchForAssessment(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new VocabPartModel(x)).ToList());
    }

    public override async Task<Response<VocabPartModel?>> Fetch(Guid? id)
    {
        var response = await _vocabPartService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new VocabPartModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _vocabPartService.Remove(new(new()
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
        var orderables = Cards.Select(x => x.Entity.VocabPart).ToList();
        return await WithWaiting("Saving...", () => _vocabPartService.SaveOrder(new(orderables)));
    }
}

public class VocabPartModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleVocabPartChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(VocabPart));

    private VocabPart _vocabPart;

    public String? Name => VocabPart.Title;

    public VocabPartModel(VocabPart vocabPart)
    {
        _vocabPart = vocabPart;
        _vocabPart.PropertyChanged += HandleVocabPartChanged;
    }

    public void Dispose()
    {
        _vocabPart.PropertyChanged -= HandleVocabPartChanged;
    }

    public Guid? Id
    {
        get => _vocabPart.Id;
        set => _vocabPart.Id = value;
    }

    public Int32? Ordinal
    {
        get => _vocabPart.Ordinal;
        set => _vocabPart.Ordinal = value;
    }

    public VocabPart VocabPart
    {
        get => _vocabPart;
        set => SetProperty(ref _vocabPart, value);
    }
}