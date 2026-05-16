namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SurveyEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISurveyService SurveyService { get; set; } = null!;

    public SurveyEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal");

        Model = new(Path, Id, IsNew, portalId, EventBus, Navigator, SurveyService);
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

public class SurveyEditModel : EditModel<Survey>, IHandle<SurveySaved>, IHandle<SurveyRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly ISurveyService _surveyService;

    public SurveyEditModel(String? path, Guid? id, Boolean isNew, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        ISurveyService surveyService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _navigator = navigator;
        _surveyService = surveyService;

        eventBus.Subscribe(this);
    }

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Handle(SurveySaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(SurveyRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await FetchContentStatusNames();
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetSurvey(new()
            {
                PortalId = _portalId,
                Title = "New Survey",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                AssignmentKind = Survey.AssignmentKinds.Automatic,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () =>
                _surveyService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetSurvey(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _surveyService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/survey-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _surveyService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _surveyService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    private void SetSurvey(Survey? survey)
    {
        Entity = survey;
        _navigator.UpdateTitle(_path, Entity?.Title);
    }
}