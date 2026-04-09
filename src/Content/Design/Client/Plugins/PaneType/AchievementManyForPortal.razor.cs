namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class AchievementManyForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IAchievementService AchievementService { get; set; } = null!;

    public AchievementManyForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, AchievementService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class AchievementManyForPortalModel : ManyModel<AchievementModel>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly IAchievementService _achievementService;
    private readonly Guid? _portalId;

    public AchievementManyForPortalModel(IEventBus eventBus, IScrollService scrollService, IAchievementService achievementService, Guid? portalId)
        : base(scrollService)
    {
        _achievementService = achievementService;

        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(AchievementAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(AchievementSaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(AchievementRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public async Task Initialize()
    {
        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _achievementService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new AchievementModel(x)));
    }

    public override async Task Create()
    {
        var achievement = new Achievement
        {
            Id = Guid.NewGuid(),
            Title = "New Achievement",
            PortalId = _portalId,
        };

        var form = await CreateForm(new(achievement));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_portalId);
    }

    public override async Task<Response<AchievementModel?>> Fetch(Guid? id)
    {
        var response = await _achievementService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new AchievementModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<AchievementModel?>> Add(FormModel<AchievementModel> form)
    {
        var response = await _achievementService.Add(new(form.Entity.Achievement));

        return response.Ok
            ? new(new AchievementModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<AchievementModel> form)
    {
        var achievement = form.Entity.Achievement;

        return await _achievementService.Save(new(achievement));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _achievementService.Remove(new(new()
        {
            Id = id,
            PortalId = _portalId,
        }));
    }
}

public class AchievementModel : Observable, IDisposable, INamed
{
    private void HandleAchievementChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Achievement));

    private Achievement _achievement;

    public String? Name => Achievement.Title;

    public AchievementModel(Achievement achievement)
    {
        _achievement = achievement;
        _achievement.PropertyChanged += HandleAchievementChanged;
    }

    public void Dispose()
    {
        _achievement.PropertyChanged -= HandleAchievementChanged;
    }

    public Guid? Id
    {
        get => _achievement.Id;
        set => _achievement.Id = value;
    }

    public Achievement Achievement
    {
        get => _achievement;
        set => SetProperty(ref _achievement, value);
    }
}