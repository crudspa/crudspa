namespace Crudspa.Samples.Composer.Client.Plugins.PaneType;

public partial class ComposerContactFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IComposerContactService ComposerContactService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public ComposerContactFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ComposerContactService);
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
        Navigator.GoTo($"{Path}/composer-contact-{Guid.NewGuid():D}?state=new");
    }
}

public class ComposerContactFindModel : FindModel<ComposerContactSearch, ComposerContact>,
    IHandle<ComposerContactAdded>, IHandle<ComposerContactSaved>, IHandle<ComposerContactRemoved>
{
    private readonly IComposerContactService _composerContactService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public ComposerContactFindModel(IEventBus eventBus, IScrollService scrollService, IComposerContactService composerContactService)
        : base(scrollService)
    {
        _composerContactService = composerContactService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "First Name",
            "Last Name",
            "Username",
        ];
    }

    public async Task Handle(ComposerContactAdded payload) => await Refresh();

    public async Task Handle(ComposerContactSaved payload) => await Refresh();

    public async Task Handle(ComposerContactRemoved payload) => await Refresh();

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

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<ComposerContactSearch>(Search);
        var response = await WithWaiting("Searching...", () => _composerContactService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _composerContactService.Remove(new(new() { Id = id })));
    }
}