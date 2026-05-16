namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SurveyDesign : IPaneDesign, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public String? Path { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public EventCallback ConfigUpdated { get; set; }

    [Inject] public ISurveyService SurveyService { get; set; } = null!;

    public SurveyDesignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<SurveyConfig>() ?? new();
        var portalId = Path!.Id("portal");

        Model = new(config, SurveyService, portalId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public Task<Boolean> PrepareForSave()
    {
        Model.Alerts.Clear();

        if (Model.Config.SurveyId.HasValue)
            return Task.FromResult(true);

        Model.Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Survey is required." });

        return Task.FromResult(false);
    }

    public String? GetConfigJson() => Model.Config.ToJson();
}

public class SurveyDesignModel(SurveyConfig config, ISurveyService surveyService, Guid? portalId) : ScreenModel
{
    public SurveyConfig Config
    {
        get;
        set => SetProperty(ref field, value);
    } = config;

    public ObservableCollection<Named> Surveys
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        var response = await WithWaiting("Fetching...", () =>
            surveyService.FetchNames(new(new() { Id = portalId })));

        if (response.Ok)
        {
            Surveys = response.Value.ToObservable();
            Config.SurveyId ??= Surveys.FirstOrDefault()?.Id;
        }
    }
}