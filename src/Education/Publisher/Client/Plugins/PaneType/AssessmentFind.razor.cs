namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class AssessmentFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IAssessmentService AssessmentService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public AssessmentFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, AssessmentService);
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
        Navigator.GoTo($"{Path}/assessment-{Guid.NewGuid():D}?state=new");
    }
}

public class AssessmentFindModel : FindModel<AssessmentSearch, Assessment>,
    IHandle<AssessmentAdded>, IHandle<AssessmentSaved>, IHandle<AssessmentRemoved>,
    IHandle<VocabPartAdded>, IHandle<VocabPartRemoved>,
    IHandle<ListenPartAdded>, IHandle<ListenPartRemoved>,
    IHandle<ReadPartAdded>, IHandle<ReadPartRemoved>
{
    private readonly IAssessmentService _assessmentService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public AssessmentFindModel(IEventBus eventBus, IScrollService scrollService, IAssessmentService assessmentService)
        : base(scrollService)
    {
        _assessmentService = assessmentService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Name",
            "Start",
            "End",
        ];
    }

    public async Task Handle(AssessmentAdded payload) => await Refresh();

    public async Task Handle(AssessmentSaved payload) => await Refresh();

    public async Task Handle(AssessmentRemoved payload) => await Refresh();

    public async Task Handle(VocabPartAdded payload) => await Refresh();

    public async Task Handle(VocabPartRemoved payload) => await Refresh();

    public async Task Handle(ListenPartAdded payload) => await Refresh();

    public async Task Handle(ListenPartRemoved payload) => await Refresh();

    public async Task Handle(ReadPartAdded payload) => await Refresh();

    public async Task Handle(ReadPartRemoved payload) => await Refresh();

    public List<Orderable> GradeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ContentCategoryNames
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
        Search.Grades.Clear();
        Search.Status.Clear();
        Search.Categories.Clear();
        Search.AvailableStartRange.Type = DateRange.Types.Any;
        Search.AvailableEndRange.Type = DateRange.Types.Any;

        await WithMany("Initializing...",
            FetchGradeNames(),
            FetchContentStatusNames(),
            FetchContentCategoryNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<AssessmentSearch>(Search);
        var response = await WithWaiting("Searching...", () => _assessmentService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _assessmentService.Remove(new(new() { Id = id })));
    }

    public async Task FetchGradeNames()
    {
        var response = await WithAlerts(() => _assessmentService.FetchGradeNames(new()), false);
        if (response.Ok) GradeNames = response.Value.ToList();
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _assessmentService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchContentCategoryNames()
    {
        var response = await WithAlerts(() => _assessmentService.FetchContentCategoryNames(new()), false);
        if (response.Ok) ContentCategoryNames = response.Value.ToList();
    }
}