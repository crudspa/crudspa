using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class UnitList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IUnitService UnitService { get; set; } = null!;

    public UnitListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, UnitService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class UnitListModel : ListOrderablesModel<UnitModel>,
    IHandle<UnitAdded>, IHandle<UnitSaved>, IHandle<UnitRemoved>, IHandle<UnitsReordered>,
    IHandle<LessonAdded>, IHandle<LessonRemoved>,
    IHandle<UnitBookAdded>, IHandle<UnitBookRemoved>,
    IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly IUnitService _unitService;

    public CopyModel CopyModel;

    public UnitListModel(IEventBus eventBus, IScrollService scrollService, IUnitService unitService)
        : base(scrollService)
    {
        _unitService = unitService;

        CopyModel = new(scrollService);

        eventBus.Subscribe(this);
    }

    public async Task Handle(UnitAdded payload) => await Replace(payload.Id);

    public async Task Handle(UnitSaved payload) => await Replace(payload.Id);

    public async Task Handle(UnitRemoved payload) => await Rid(payload.Id);

    public async Task Handle(UnitsReordered payload) => await Refresh();

    public async Task Handle(AchievementSaved payload) => await Refresh(false);

    public async Task Handle(AchievementRemoved payload) => await Refresh(false);

    public async Task Handle(LessonAdded payload)
    {
        if (payload.UnitId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.UnitId)))
            await Replace(payload.UnitId);
    }

    public async Task Handle(LessonRemoved payload)
    {
        if (payload.UnitId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.UnitId)))
            await Replace(payload.UnitId);
    }

    public async Task Handle(UnitBookAdded payload)
    {
        if (payload.UnitId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.UnitId)))
            await Replace(payload.UnitId);
    }

    public async Task Handle(UnitBookRemoved payload)
    {
        if (payload.UnitId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.UnitId)))
            await Replace(payload.UnitId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request(new());
        var response = await WithWaiting("Fetching...", () => _unitService.FetchAll(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new UnitModel(x)).ToList());
    }

    public override async Task<Response<UnitModel?>> Fetch(Guid? id)
    {
        var response = await _unitService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new UnitModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _unitService.Remove(new(new() { Id = id }));
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Unit).ToList();
        return await WithWaiting("Saving...", () => _unitService.SaveOrder(new(orderables)));
    }

    public async Task StartCopy(Guid? id)
    {
        var original = Cards.First(x => x.Entity.Id.Equals(id)).Entity;

        CopyModel.Copy.ExistingId = original.Id;
        CopyModel.Copy.NewName = original.Name;

        await CopyModel.Show();
    }

    public async Task Copy()
    {
        await CopyModel.Hide();
        await WithWaiting("Copying...", () => _unitService.Copy(new(CopyModel.Copy)));
    }
}

public class UnitModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleUnitChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Unit));

    private Unit _unit;

    public String? Name => Unit.Title;

    public UnitModel(Unit unit)
    {
        _unit = unit;
        _unit.PropertyChanged += HandleUnitChanged;
    }

    public void Dispose()
    {
        _unit.PropertyChanged -= HandleUnitChanged;
    }

    public Guid? Id
    {
        get => _unit.Id;
        set => _unit.Id = value;
    }

    public Int32? Ordinal
    {
        get => _unit.Ordinal;
        set => _unit.Ordinal = value;
    }

    public Unit Unit
    {
        get => _unit;
        set => SetProperty(ref _unit, value);
    }
}