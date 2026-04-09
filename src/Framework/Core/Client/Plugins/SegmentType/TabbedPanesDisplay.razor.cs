using Crudspa.Framework.Core.Shared.Contracts.Config.SegmentType;

namespace Crudspa.Framework.Core.Client.Plugins.SegmentType;

public partial class TabbedPanesDisplay : ISegmentDisplay
{
    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public IEnumerable<NavPane>? Panes { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;

    public TabsModel Tabs { get; set; } = null!;
    public TabbedPanesConfig Config { get; set; } = null!;
    public Boolean IsNew { get; set; }

    protected override Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<TabbedPanesConfig>() ?? new();
        IsNew = HasState(Path!, out var state) && state.IsBasically("new");

        Tabs = new(Navigator, Path!, queryKey: "pane", lockOnNew: IsNew);

        var tabs = Panes!.OrderBy(x => x.Ordinal).Select(pane => new Tab
        {
            Key = pane.Key!,
            Title = pane.Name!,
            Lazy = pane.Ordinal > 0,
            PaneTypeDisplayView = pane.TypeDisplayView,
            Id = Id ?? Path.Id(),
            IsNew = IsNew,
            ConfigJson = pane.ConfigJson,
        });

        Tabs.Initialize(tabs);

        return Task.CompletedTask;
    }

    private Boolean HasState(String path, out String? state)
    {
        var parameters = Navigator.GetQueryParameters(path);
        var stateParameter = parameters["state"];

        state = stateParameter ?? null;

        return stateParameter is not null;
    }
}