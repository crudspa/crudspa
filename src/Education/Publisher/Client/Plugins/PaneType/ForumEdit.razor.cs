using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;
using ForumRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.ForumRemoved;
using ForumSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.ForumSaved;
using IForumService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IForumService;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ForumEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IForumService ForumService { get; set; } = null!;

    public ForumEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ForumService);
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

public class ForumEditModel : EditModel<Forum>,
    IHandle<ForumSaved>, IHandle<ForumRemoved>,
    IHandle<DistrictAdded>, IHandle<DistrictSaved>, IHandle<DistrictRemoved>,
    IHandle<SchoolAdded>, IHandle<SchoolSaved>, IHandle<SchoolRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IForumService _forumService;

    public ForumEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IForumService forumService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _forumService = forumService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ForumSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ForumRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(DistrictAdded payload) => await FetchDistrictNames();

    public async Task Handle(DistrictSaved payload) => await FetchDistrictNames();

    public async Task Handle(DistrictRemoved payload) => await FetchDistrictNames();

    public async Task Handle(SchoolAdded payload) => await FetchSchoolNames();

    public async Task Handle(SchoolSaved payload) => await FetchSchoolNames();

    public async Task Handle(SchoolRemoved payload) => await FetchSchoolNames();

    public List<Orderable> BodyTemplateNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> DistrictNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Named> SchoolNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchBodyTemplateNames(),
            FetchDistrictNames(),
            FetchSchoolNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetForum(new()
            {
                Name = "New Forum",
                Description = String.Empty,
                BodyTemplateId = BodyTemplateNames.MinBy(x => x.Ordinal)?.Id,
                Pinned = false,
                InnovatorsOnly = false,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _forumService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetForum(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _forumService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/forum-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _forumService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchBodyTemplateNames()
    {
        var response = await WithAlerts(() => _forumService.FetchBodyTemplateNames(new()), false);
        if (response.Ok) BodyTemplateNames = response.Value.ToList();
    }

    public async Task FetchDistrictNames()
    {
        var response = await WithAlerts(() => _forumService.FetchDistrictNames(new()), false);
        if (response.Ok) DistrictNames = response.Value.ToObservable();
    }

    public async Task FetchSchoolNames()
    {
        var response = await WithAlerts(() => _forumService.FetchSchoolNames(new()), false);
        if (response.Ok) SchoolNames = response.Value.ToObservable();
    }

    private void SetForum(Forum forum)
    {
        Entity = forum;
        _navigator.UpdateTitle(_path, Entity.Name!);
    }
}