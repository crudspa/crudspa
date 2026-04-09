using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;
using IAchievementService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IAchievementService;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class AchievementEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IAchievementService AchievementService { get; set; } = null!;

    public AchievementEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, AchievementService);
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

public class AchievementEditModel : EditModel<Achievement>,
    IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IAchievementService _achievementService;

    public AchievementEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IAchievementService achievementService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _achievementService = achievementService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(AchievementSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(AchievementRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<Orderable> RarityNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchRarityNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var achievement = new Achievement
            {
                Title = "New Achievement",
                RarityId = RarityNames.MinBy(x => x.Ordinal)?.Id,
                VisibleToStudents = true,
            };

            SetAchievement(achievement);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _achievementService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetAchievement(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _achievementService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/achievement-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _achievementService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchRarityNames()
    {
        var response = await WithAlerts(() => _achievementService.FetchRarityNames(new()), false);
        if (response.Ok) RarityNames = response.Value.ToList();
    }

    private void SetAchievement(Achievement achievement)
    {
        Entity = achievement;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}