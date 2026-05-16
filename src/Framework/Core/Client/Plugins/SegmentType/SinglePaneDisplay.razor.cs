using Crudspa.Framework.Core.Shared.Contracts.Config.SegmentType;

namespace Crudspa.Framework.Core.Client.Plugins.SegmentType;

public partial class SinglePaneDisplay : ISegmentDisplay
{
    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public IEnumerable<NavPane>? Panes { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;

    public NavPane? Pane { get; set; }
    public Guid? PaneId { get; set; }
    public Guid? DisplayId { get; set; }
    public SinglePaneConfig Config { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<SinglePaneConfig>() ?? new();
        var hasState = HasState(Path!, out var state);

        var pane = Panes!.OrderBy(x => x.Ordinal).First();

        PaneId = pane.Id;
        DisplayId = Id ?? Path.Id();
        pane.Id = DisplayId;
        pane.IsNew = hasState && state.IsBasically("new");

        Pane = pane;

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