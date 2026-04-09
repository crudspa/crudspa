using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;
using AchievementAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementAdded;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;
using IAchievementService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IAchievementService;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class AchievementFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IAchievementService AchievementService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public AchievementFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, AchievementService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void AddNew()
    {
        Navigator.GoTo($"{Path}/achievement-{Guid.NewGuid():D}?state=new");
    }
}

public class AchievementFindModel : FindModel<AchievementSearch, Achievement>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly IAchievementService _achievementService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public AchievementFindModel(IEventBus eventBus, IScrollService scrollService, IAchievementService achievementService)
        : base(scrollService)
    {
        _achievementService = achievementService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Title",
            "Rarity",
        ];
    }

    public async Task Handle(AchievementAdded payload) => await Refresh();

    public async Task Handle(AchievementSaved payload) => await Refresh();

    public async Task Handle(AchievementRemoved payload) => await Refresh();

    public List<Orderable> RarityNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = true;
        Search.Rarities.Clear();

        await WithMany("Initializing...",
            FetchRarityNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<AchievementSearch>(Search);
        var response = await WithWaiting("Searching...", () => _achievementService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _achievementService.Remove(new(new() { Id = id })));
    }

    public async Task FetchRarityNames()
    {
        var response = await WithAlerts(() => _achievementService.FetchRarityNames(new()), false);
        if (response.Ok) RarityNames = response.Value.ToList();
    }
}