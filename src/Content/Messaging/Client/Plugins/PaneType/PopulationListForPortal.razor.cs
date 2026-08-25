namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class PopulationListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPopulationService PopulationService { get; set; } = null!;

    public PopulationListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PopulationService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PopulationListForPortalModel : ListModel<PopulationModel>,
    IHandle<PopulationAdded>, IHandle<PopulationSaved>, IHandle<PopulationRemoved>
{
    private readonly IPopulationService _populationService;
    private readonly Guid? _portalId;

    public PopulationListForPortalModel(IEventBus eventBus, IScrollService scrollService, IPopulationService populationService, Guid? portalId)
        : base(scrollService)
    {
        _populationService = populationService;

        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PopulationAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(PopulationSaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(PopulationRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _populationService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new PopulationModel(x)).ToList());
    }

    public override async Task<Response<PopulationModel?>> Fetch(Guid? id)
    {
        var response = await _populationService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new PopulationModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _populationService.Remove(new(new()
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

public class PopulationModel : Observable, IDisposable, INamed
{
    private void HandlePopulationChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Population));

    private Population _population;

    public String? Name => Population.Name;

    public PopulationModel(Population population)
    {
        _population = population;
        _population.PropertyChanged += HandlePopulationChanged;
    }

    public void Dispose()
    {
        _population.PropertyChanged -= HandlePopulationChanged;
    }

    public Guid? Id
    {
        get => _population.Id;
        set => _population.Id = value;
    }

    public Population Population
    {
        get => _population;
        set => SetProperty(ref _population, value);
    }
}