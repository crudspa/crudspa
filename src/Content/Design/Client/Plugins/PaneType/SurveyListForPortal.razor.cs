namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SurveyListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISurveyService SurveyService { get; set; } = null!;

    public SurveyListForPortalModel Model { get; set; } = null!;

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

public class SurveyListForPortalModel : ListModel<SurveyModel>,
    IHandle<SurveyAdded>, IHandle<SurveySaved>, IHandle<SurveyRemoved>,
    IHandle<SurveyPartAdded>, IHandle<SurveyPartRemoved>
{
    private readonly ISurveyService _surveyService;
    private readonly Guid? _portalId;

    public SurveyListForPortalModel(IEventBus eventBus, IScrollService scrollService, ISurveyService surveyService, Guid? portalId)
        : base(scrollService)
    {
        _surveyService = surveyService;
        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SurveyAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(SurveySaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(SurveyRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public async Task Handle(SurveyPartAdded payload) => await Refresh(false);

    public async Task Handle(SurveyPartRemoved payload) => await Refresh(false);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var response = await WithWaiting("Fetching...", () =>
            _surveyService.FetchForPortal(new(new() { Id = _portalId })), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new SurveyModel(x)).ToList());
    }

    public override async Task<Response<SurveyModel?>> Fetch(Guid? id)
    {
        var response = await _surveyService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new SurveyModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _surveyService.Remove(new(new()
        {
            Id = id,
            PortalId = _portalId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_portalId);
    }
}

public class SurveyModel : Observable, IDisposable, INamed
{
    private void HandleSurveyChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Survey));

    private Survey _survey;

    public SurveyModel(Survey survey)
    {
        _survey = survey;
        _survey.PropertyChanged += HandleSurveyChanged;
    }

    public String? Name => Survey.Title;

    public Guid? Id
    {
        get => _survey.Id;
        set => _survey.Id = value;
    }

    public Survey Survey
    {
        get => _survey;
        set => SetProperty(ref _survey, value);
    }

    public void Dispose()
    {
        _survey.PropertyChanged -= HandleSurveyChanged;
    }
}