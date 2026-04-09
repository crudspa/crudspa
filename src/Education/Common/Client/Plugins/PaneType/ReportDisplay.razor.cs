using Crudspa.Education.Common.Shared.Contracts.Config;

namespace Crudspa.Education.Common.Client.Plugins.PaneType;

public partial class ReportDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IReportService ReportService { get; set; } = null!;

    public ReportDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<ReportConfig>();

        if (config is not null && config.IdSource == ReportConfig.IdSources.SpecificReport && config.ReportId.HasSomething())
            Id = config.ReportId;

        Model = new(Path, Id, Navigator, ReportService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ReportDisplayModel(String? path, Guid? id, INavigator navigator, IReportService reportService) : ScreenModel
{
    public Report? Report
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Fetching...", () => reportService.Fetch(new(new() { Id = id })));

        if (response.Ok)
            SetReport(response.Value);
    }

    private void SetReport(Report report)
    {
        Report = report;
        navigator.UpdateTitle(path, Report.Name!);
    }
}