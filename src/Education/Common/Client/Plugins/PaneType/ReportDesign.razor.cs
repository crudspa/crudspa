using Crudspa.Education.Common.Shared.Contracts.Config;

namespace Crudspa.Education.Common.Client.Plugins.PaneType;

public partial class ReportDesign : IPaneDesign, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }

    [Parameter] public String? Path { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public EventCallback ConfigUpdated { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IReportService ReportService { get; set; } = null!;

    public ReportDesignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<ReportConfig>() ?? new();

        var portalId = Path!.Id("portal");

        Model = new(config, ReportService, portalId);
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

public class ReportDesignModel(ReportConfig config, IReportService reportService, Guid? portalId) : ScreenModel
{
    public ReportConfig Config
    {
        get;
        set => SetProperty(ref field, value);
    } = config;

    public ObservableCollection<Named> Reports
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        var request = new Request<Portal>(new() { Id = portalId });
        var response = await WithWaiting("Fetching...", () => reportService.FetchReportNames(request));

        if (response.Ok)
        {
            Reports = response.Value.ToObservable();
            Config.ReportId ??= Reports.FirstOrDefault()?.Id;
        }
    }
}