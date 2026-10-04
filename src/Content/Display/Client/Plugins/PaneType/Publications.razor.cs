using Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class Publications : IPaneDisplay, IDisposable
{
    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public IList<PublicationSummary>? PreviewPublications { get; set; }
    [Inject] public IPublicationRunService PublicationService { get; set; } = null!;

    public PublicationsModel Model { get; private set; } = null!;

    private Boolean _loaded;

    protected override async Task OnParametersSetAsync()
    {
        if (Model is null)
        {
            Model = new(PublicationService, new());
            Model.PropertyChanged += HandleChanged;
        }
        Model.Config = ConfigJson.FromJson<PublicationsConfig>() ?? new();
        if (PreviewPublications is not null)
            Model.UsePreview(PreviewPublications);
        else if (!_loaded)
        {
            await Model.Refresh();
            _loaded = true;
        }
    }

    private void HandleChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        Model.PropertyChanged -= HandleChanged;
        Model.Dispose();
    }
}

public class PublicationsModel(IPublicationRunService service, PublicationsConfig config) : ScreenModel
{
    public PublicationsConfig Config { get; set; } = config;
    public String Search { get; set; } = String.Empty;
    public String GroupFilter { get; set; } = String.Empty;
    public IList<PublicationSummary> Publications { get; private set; } = [];
    public void UsePreview(IList<PublicationSummary> publications) => Publications = publications;

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading publications...", () => service.FetchAll(new()));
        if (response.Ok) Publications = response.Value;
    }

    public IEnumerable<PublicationSummary> Available => Publications.Where(x => !Config.GroupId.HasValue || x.GroupId == Config.GroupId);

    public IEnumerable<PublicationSummary> Results
    {
        get
        {
            var result = Available.Where(x => (GroupFilter.Length == 0 || x.GroupId?.ToString("D") == GroupFilter)
                && (String.IsNullOrWhiteSpace(Search) || new[] { x.Title, x.Citation, x.Venue, x.Sample, x.Timeline, x.Outcome, x.Intervention }
                    .Any(value => value?.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase) == true)));
            return Config.SortOrder == PublicationsConfig.SortOrders.Newest
                ? result.OrderByDescending(x => x.Year.HasValue).ThenByDescending(x => x.Year).ThenBy(x => x.Ordinal).ThenBy(x => x.Id)
                : result.OrderBy(x => x.Ordinal).ThenBy(x => x.Id);
        }
    }

    public IEnumerable<IGrouping<Guid?, PublicationSummary>> Sections => Config.Grouped
        ? Results.GroupBy(x => x.GroupId).OrderBy(x => x.Min(p => p.GroupOrdinal ?? Int32.MaxValue)).ThenBy(x => x.First().GroupName)
        : Results.GroupBy(_ => (Guid?)null);

    public static String ResourceUrl(PublicationResourceSummary resource) => resource.Url ?? (resource.PdfFile.Id.HasValue ? resource.PdfFile.FetchUrl() : resource.ImageFile.FetchUrl());

    public static String ResourceLabel(PublicationResourceSummary resource) => resource.Label.HasSomething() ? resource.Label!
        : resource.TypeName switch
        {
            "Paper" => "Read paper",
            "Registration" => "Study registration",
            "Replication data" => "Replication data",
            "Research brief" => "Research brief",
            _ => resource.PdfFile.Name ?? resource.ImageFile.Name ?? resource.TypeName ?? "Open resource",
        };
}