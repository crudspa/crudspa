namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class CommunityEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ICommunityService CommunityService { get; set; } = null!;

    public CommunityEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ScrollService, CommunityService);
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

public class CommunityEditModel : EditModel<Community>, IHandle<CommunitySaved>, IHandle<CommunityRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ICommunityService _communityService;

    public ModalModel AddStewardsModel { get; set; }

    public CommunityEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService,
        ICommunityService communityService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _communityService = communityService;

        AddStewardsModel = new(scrollService);

        eventBus.Subscribe(this);
    }

    public async Task Handle(CommunitySaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(CommunityRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public ObservableCollection<Selectable> SelectableStewards
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetCommunity(new()
            {
                Name = "New Community",
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _communityService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetCommunity(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _communityService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/community-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _communityService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task ShowSelectableStewards()
    {
        var response = await WithWaiting("Fetching contacts...", () => _communityService.FetchDistrictContacts(new(new() { Id = _id })));

        if (response.Ok)
        {
            var stewards = response.Value;
            stewards.RemoveWhere(x => Entity!.CommunityStewards.HasAny(y => y.DistrictContactId.Equals(x.Id)));
            SelectableStewards = stewards.ToObservable();
        }

        await AddStewardsModel.Show();
    }

    public void AddStewards()
    {
        AddStewardsModel.Hide();

        foreach (var steward in SelectableStewards.Where(x => x.Selected == true))
        {
            Entity!.CommunityStewards.Add(new()
            {
                DistrictContactId = steward.Id,
                Name = steward.Name,
                CommunityId = Entity.Id,
            });
        }
    }

    public void RemoveSteward(Guid? stewardId)
    {
        Entity!.CommunityStewards.RemoveWhere(x => x.Id.Equals(stewardId));
    }

    private void SetCommunity(Community community)
    {
        Entity = community;
        _navigator.UpdateTitle(_path, Entity.Name!);
    }
}