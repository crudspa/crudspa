using MemberSearch = Crudspa.Content.Design.Shared.Contracts.Data.MemberSearch;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class MemberFindForMembership : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IMemberService MemberService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public MemberFindForMembershipModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, MemberService, Id);
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
        Navigator.GoTo($"{Path}/member-{Guid.NewGuid():D}?state=new");
    }
}

public class MemberFindForMembershipModel : FindModel<MemberSearch, Member>,
    IHandle<MemberAdded>, IHandle<MemberSaved>, IHandle<MemberRemoved>
{
    private readonly IMemberService _memberService;
    private readonly Guid? _membershipId;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public MemberFindForMembershipModel(IEventBus eventBus, IScrollService scrollService, IMemberService memberService, Guid? membershipId)
        : base(scrollService)
    {
        _memberService = memberService;
        _membershipId = membershipId;
        eventBus.Subscribe(this);

        _sorts =
        [
            "First",
            "Last",
        ];
    }

    public async Task Handle(MemberAdded payload) => await Refresh();

    public async Task Handle(MemberSaved payload) => await Refresh();

    public async Task Handle(MemberRemoved payload) => await Refresh();

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.ParentId = _membershipId;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = true;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<MemberSearch>(Search);
        var response = await WithWaiting("Searching...", () => _memberService.SearchForMembership(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _memberService.Remove(new(new() { Id = id })));
    }
}