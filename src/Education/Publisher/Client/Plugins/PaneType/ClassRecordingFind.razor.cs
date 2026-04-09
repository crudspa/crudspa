namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ClassRecordingFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IClassRecordingService ClassRecordingService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public ClassRecordingFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ClassRecordingService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ClassRecordingFindModel : FindModel<ClassRecordingSearch, ClassRecording>,
    IHandle<ClassRecordingAdded>, IHandle<ClassRecordingSaved>, IHandle<ClassRecordingRemoved>
{
    private readonly IClassRecordingService _classRecordingService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public ClassRecordingFindModel(IEventBus eventBus, IScrollService scrollService, IClassRecordingService classRecordingService)
        : base(scrollService)
    {
        _classRecordingService = classRecordingService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Uploaded",
            "Teacher",
        ];
    }

    public async Task Handle(ClassRecordingAdded payload) => await Refresh();

    public async Task Handle(ClassRecordingSaved payload) => await Refresh();

    public async Task Handle(ClassRecordingRemoved payload) => await Refresh();

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
        Search.Sort.Ascending = false;
        Search.UploadedRange.Type = DateRange.Types.Any;

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<ClassRecordingSearch>(Search);
        var response = await WithWaiting("Searching...", () => _classRecordingService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }
}