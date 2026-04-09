using AchievementAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementAdded;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class UnitEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IUnitService UnitService { get; set; } = null!;

    public UnitEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, UnitService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class UnitEditModel : EditModel<Unit>,
    IHandle<UnitSaved>, IHandle<UnitRemoved>, IHandle<UnitsReordered>, IHandle<UnitAdded>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IUnitService _unitService;

    public UnitEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IUnitService unitService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _unitService = unitService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(UnitAdded payload)
    {
        await FetchUnitNames();
    }

    public async Task Handle(UnitSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();

        await FetchUnitNames();
    }

    public async Task Handle(UnitRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        await FetchUnitNames();
    }

    public async Task Handle(UnitsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _unitService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title!);
        }
    }

    public async Task Handle(AchievementAdded payload) => await FetchAchievementNames();

    public async Task Handle(AchievementSaved payload) => await FetchAchievementNames();

    public async Task Handle(AchievementRemoved payload) => await FetchAchievementNames();

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> GradeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Orderable> UnitNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> AchievementNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchGradeNames(),
            FetchUnitNames(),
            FetchAchievementNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetUnit(new()
            {
                Title = "New Unit",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                GradeId = GradeNames.MinBy(x => x.Ordinal)?.Id,
                Treatment = true,
                Control = true,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _unitService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetUnit(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _unitService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/unit-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _unitService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _unitService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchGradeNames()
    {
        var response = await WithAlerts(() => _unitService.FetchGradeNames(new()), false);
        if (response.Ok) GradeNames = response.Value.ToList();
    }

    public async Task FetchUnitNames()
    {
        var response = await WithAlerts(() => _unitService.FetchUnitNames(new()), false);
        if (response.Ok) UnitNames = response.Value.ToObservable();
    }

    public async Task FetchAchievementNames()
    {
        var response = await WithAlerts(() => _unitService.FetchAchievementNames(new()), false);
        if (response.Ok) AchievementNames = response.Value.ToObservable();
    }

    private void SetUnit(Unit unit)
    {
        Entity = unit;
        _navigator.UpdateTitle(_path, Entity.Title!);
    }
}