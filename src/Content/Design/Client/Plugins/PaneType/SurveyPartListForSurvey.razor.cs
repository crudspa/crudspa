namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SurveyPartListForSurvey : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISurveyService SurveyService { get; set; } = null!;

    public SurveyPartListForSurveyModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SurveyService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SurveyPartListForSurveyModel : ListOrderablesModel<SurveyPartModel>,
    IHandle<SurveyPartAdded>, IHandle<SurveyPartSaved>, IHandle<SurveyPartRemoved>, IHandle<SurveyPartsReordered>,
    IHandle<SurveyQuestionAdded>, IHandle<SurveyQuestionRemoved>
{
    private readonly ISurveyService _surveyService;
    private readonly Guid? _surveyId;

    public SurveyPartListForSurveyModel(IEventBus eventBus, IScrollService scrollService, ISurveyService surveyService, Guid? surveyId)
        : base(scrollService)
    {
        _surveyService = surveyService;
        _surveyId = surveyId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SurveyPartAdded payload) => await Replace(payload.Id, payload.SurveyId);

    public async Task Handle(SurveyPartSaved payload) => await Replace(payload.Id, payload.SurveyId);

    public async Task Handle(SurveyPartRemoved payload) => await Rid(payload.Id, payload.SurveyId);

    public async Task Handle(SurveyPartsReordered payload) => await Refresh(false);

    public async Task Handle(SurveyQuestionAdded payload) => await Refresh(false);

    public async Task Handle(SurveyQuestionRemoved payload) => await Refresh(false);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var response = await WithWaiting("Fetching...", () =>
            _surveyService.FetchParts(new(new() { Id = _surveyId })), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new SurveyPartModel(x)).ToList());
    }

    public override async Task<Response<SurveyPartModel?>> Fetch(Guid? id)
    {
        var response = await _surveyService.FetchPart(new(new() { Id = id }));

        return response.Ok
            ? new(new SurveyPartModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _surveyService.RemovePart(new(new()
        {
            Id = id,
            SurveyId = _surveyId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_surveyId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Part).ToList();
        return await WithWaiting("Saving...", () => _surveyService.SavePartOrder(new(orderables)));
    }
}

public class SurveyPartModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandlePartChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Part));

    private SurveyPart _part;

    public SurveyPartModel(SurveyPart part)
    {
        _part = part;
        _part.PropertyChanged += HandlePartChanged;
    }

    public String? Name => Part.Title;

    public Guid? Id
    {
        get => _part.Id;
        set => _part.Id = value;
    }

    public Int32? Ordinal
    {
        get => _part.Ordinal;
        set => _part.Ordinal = value;
    }

    public SurveyPart Part
    {
        get => _part;
        set => SetProperty(ref _part, value);
    }

    public void Dispose()
    {
        _part.PropertyChanged -= HandlePartChanged;
    }
}