namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class TrackDesign : IPaneDesign, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }

    [Parameter] public String? Path { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public EventCallback ConfigUpdated { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IContentPortalService ContentPortalService { get; set; } = null!;

    public TrackDesignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<TrackConfig>() ?? new();

        var portalId = Path!.Id("portal");

        Model = new(config, ContentPortalService, portalId);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public Task<Boolean> PrepareForSave() => Task.FromResult(true);

    public String? GetConfigJson() => Model.Config.ToJson();
}

public class TrackDesignModel(TrackConfig config, IContentPortalService contentPortalService, Guid? portalId) : ScreenModel
{
    public TrackConfig Config
    {
        get;
        set => SetProperty(ref field, value);
    } = config;

    public ObservableCollection<Named> Tracks
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        var request = new Request<ContentPortal>(new() { Id = portalId });
        var response = await WithWaiting("Fetching...", () => contentPortalService.FetchTrackNames(request));

        if (response.Ok)
        {
            Tracks = response.Value.ToObservable();
            Config.TrackId ??= Tracks.FirstOrDefault()?.Id;
        }
    }
}