namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class MemberEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IMemberService MemberService { get; set; } = null!;

    public MemberEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var membershipId = Path!.Id("membership");

        Model = new(Path, Id, IsNew, membershipId, EventBus, Navigator, MemberService);
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

public class MemberEditModel : EditModel<Member>,
    IHandle<MemberSaved>, IHandle<MemberRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _membershipId;
    private readonly INavigator _navigator;
    private readonly IMemberService _memberService;

    public MemberEditModel(String? path, Guid? id, Boolean isNew, Guid? membershipId,
        IEventBus eventBus,
        INavigator navigator,
        IMemberService memberService) : base(isNew)
    {
        _path = path;
        _id = id;
        _membershipId = membershipId;
        _navigator = navigator;
        _memberService = memberService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(MemberSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(MemberRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetMember(new()
            {
                MembershipId = _membershipId,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _memberService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetMember(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _memberService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/member-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _memberService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void SetMember(Member member)
    {
        Entity = member;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}