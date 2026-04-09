namespace Crudspa.Education.Common.Client.Plugins.PaneType;

public partial class ReportList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IReportService ReportService { get; set; } = null!;

    public ReportListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(ScrollService, ReportService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ReportListModel(IScrollService scrollService, IReportService reportService) : ListModel<ReportModel>(scrollService)
{
    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request(Guid.Empty);
        var response = await WithWaiting("Fetching...", () => reportService.FetchAll(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ReportModel(x)).ToList());
    }

    public override async Task<Response<ReportModel?>> Fetch(Guid? id)
    {
        await Task.CompletedTask;
        return new();
    }

    public override async Task<Response> Remove(Guid? id)
    {
        await Task.CompletedTask;
        return new();
    }
}

public class ReportModel : Observable, IDisposable, INamed, IOrderable
{
    private Report _report;

    public String? Name => Report.Name;

    public ReportModel(Report report)
    {
        _report = report;
        _report.PropertyChanged += HandleReportChanged;
    }

    public void Dispose()
    {
        _report.PropertyChanged -= HandleReportChanged;
    }

    private void HandleReportChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(Report));
    }

    public Guid? Id
    {
        get => _report.Id;
        set => _report.Id = value;
    }

    public Int32? Ordinal
    {
        get => _report.Ordinal;
        set => _report.Ordinal = value;
    }

    public Report Report
    {
        get => _report;
        set => SetProperty(ref _report, value);
    }
}