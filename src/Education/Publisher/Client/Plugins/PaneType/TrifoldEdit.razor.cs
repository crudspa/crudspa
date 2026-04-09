using AchievementAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementAdded;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class TrifoldEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ITrifoldService TrifoldService { get; set; } = null!;

    public TrifoldEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var bookId = Path!.Id("book");

        Model = new(Path, Id, IsNew, bookId, EventBus, Navigator, TrifoldService);
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

public class TrifoldEditModel : EditModel<Trifold>,
    IHandle<TrifoldSaved>, IHandle<TrifoldRemoved>, IHandle<TrifoldsReordered>,
    IHandle<AchievementAdded>, IHandle<AchievementSaved>, IHandle<AchievementRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _bookId;
    private readonly INavigator _navigator;
    private readonly ITrifoldService _trifoldService;

    public TrifoldEditModel(String? path, Guid? id, Boolean isNew, Guid? bookId,
        IEventBus eventBus,
        INavigator navigator,
        ITrifoldService trifoldService) : base(isNew)
    {
        _path = path;
        _id = id;
        _bookId = bookId;
        _navigator = navigator;
        _trifoldService = trifoldService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(TrifoldSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(TrifoldRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(TrifoldsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _trifoldService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title);
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

    public ObservableCollection<Named> AchievementNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> BinderTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchAchievementNames(),
            FetchBinderTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetTrifold(new()
            {
                BookId = _bookId,
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                Binder = new()
                {
                    TypeId = BinderTypeNames.MinBy(x => x.Ordinal)?.Id,
                },
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _trifoldService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetTrifold(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _trifoldService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/trifold-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _trifoldService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _trifoldService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchAchievementNames()
    {
        var response = await WithAlerts(() => _trifoldService.FetchAchievementNames(new()), false);
        if (response.Ok) AchievementNames = response.Value.ToObservable();
    }

    public async Task FetchBinderTypeNames()
    {
        var response = await WithAlerts(() => _trifoldService.FetchBinderTypeNames(new()), false);
        if (response.Ok) BinderTypeNames = response.Value.ToList();
    }

    private void SetTrifold(Trifold trifold)
    {
        Entity = trifold;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}