using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;
using ForumAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.ForumAdded;
using ForumRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.ForumRemoved;
using ForumSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.ForumSaved;
using IForumService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IForumService;
using PostAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.PostAdded;
using PostRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.PostRemoved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ForumFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IForumService ForumService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public ForumFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ForumService);
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
        Navigator.GoTo($"{Path}/forum-{Guid.NewGuid():D}?state=new");
    }
}

public class ForumFindModel : FindModel<ForumSearch, Forum>,
    IHandle<ForumAdded>, IHandle<ForumSaved>, IHandle<ForumRemoved>,
    IHandle<DistrictAdded>, IHandle<DistrictSaved>, IHandle<DistrictRemoved>,
    IHandle<PostAdded>, IHandle<PostRemoved>
{
    private readonly IForumService _forumService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public ForumFindModel(IEventBus eventBus, IScrollService scrollService, IForumService forumService)
        : base(scrollService)
    {
        _forumService = forumService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Name",
            "Pinned",
        ];
    }

    public async Task Handle(ForumAdded payload) => await Refresh();

    public async Task Handle(ForumSaved payload) => await Refresh();

    public async Task Handle(ForumRemoved payload) => await Refresh();

    public async Task Handle(DistrictAdded payload) => await FetchDistrictNames();

    public async Task Handle(DistrictSaved payload) => await FetchDistrictNames();

    public async Task Handle(DistrictRemoved payload) => await FetchDistrictNames();

    public async Task Handle(PostAdded payload) => await Refresh();

    public async Task Handle(PostRemoved payload) => await Refresh();

    public ObservableCollection<Named> DistrictNames
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
        Search.Districts.Clear();

        await WithMany("Initializing...",
            FetchDistrictNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<ForumSearch>(Search);
        var response = await WithWaiting("Searching...", () => _forumService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _forumService.Remove(new(new() { Id = id })));
    }

    public async Task FetchDistrictNames()
    {
        var response = await WithAlerts(() => _forumService.FetchDistrictNames(new()), false);
        if (response.Ok) DistrictNames = response.Value.ToObservable();
    }
}