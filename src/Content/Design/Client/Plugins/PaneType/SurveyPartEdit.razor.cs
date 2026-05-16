namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SurveyPartEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISurveyService SurveyService { get; set; } = null!;

    public SurveyPartEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var surveyId = Path!.Id("survey");

        Model = new(Path, Id, IsNew, surveyId, EventBus, Navigator, SurveyService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
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

public class SurveyPartEditModel : EditModel<SurveyPart>, IHandle<SurveyPartSaved>, IHandle<SurveyPartRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _surveyId;
    private readonly INavigator _navigator;
    private readonly ISurveyService _surveyService;

    public SurveyPartEditModel(String? path, Guid? id, Boolean isNew, Guid? surveyId,
        IEventBus eventBus,
        INavigator navigator,
        ISurveyService surveyService) : base(isNew)
    {
        _path = path;
        _id = id;
        _surveyId = surveyId;
        _navigator = navigator;
        _surveyService = surveyService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SurveyPartSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(SurveyPartRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetPart(new()
            {
                SurveyId = _surveyId,
                Title = "New Part",
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () =>
                _surveyService.FetchPart(new(new() { Id = _id })));

            if (response.Ok)
                SetPart(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _surveyService.AddPart(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/survey-part-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _surveyService.SavePart(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void SetPart(SurveyPart? part)
    {
        Entity = part;
        _navigator.UpdateTitle(_path, Entity?.Title);
    }
}