namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class AssessmentEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IAssessmentService AssessmentService { get; set; } = null!;

    public AssessmentEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, AssessmentService);
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

public class AssessmentEditModel : EditModel<Assessment>,
    IHandle<AssessmentSaved>, IHandle<AssessmentRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IAssessmentService _assessmentService;

    public AssessmentEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IAssessmentService assessmentService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _assessmentService = assessmentService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(AssessmentSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(AssessmentRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<Orderable> ContentCategoryNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> GradeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentCategoryNames(),
            FetchContentStatusNames(),
            FetchGradeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var assessment = new Assessment
            {
                Name = "New Assessment",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                GradeId = GradeNames.MinBy(x => x.Ordinal)?.Id,
            };

            SetAssessment(assessment);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _assessmentService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetAssessment(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _assessmentService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/assessment-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _assessmentService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentCategoryNames()
    {
        var response = await WithAlerts(() => _assessmentService.FetchContentCategoryNames(new()), false);
        if (response.Ok) ContentCategoryNames = response.Value.ToList();
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _assessmentService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchGradeNames()
    {
        var response = await WithAlerts(() => _assessmentService.FetchGradeNames(new()), false);
        if (response.Ok) GradeNames = response.Value.ToList();
    }

    private void SetAssessment(Assessment assessment)
    {
        Entity = assessment;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}