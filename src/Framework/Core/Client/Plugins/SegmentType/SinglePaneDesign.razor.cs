using Crudspa.Framework.Core.Shared.Contracts.Config.SegmentType;

namespace Crudspa.Framework.Core.Client.Plugins.SegmentType;

public partial class SinglePaneDesign : ISegmentDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public ObservableCollection<Pane>? Panes { get; set; }
    [Parameter] public BatchModel<Pane>? PanesModel { get; set; }
    [Parameter] public List<Named>? PermissionNames { get; set; }
    [Parameter] public List<PaneTypeFull>? PaneTypes { get; set; }
    [Parameter] public Action? AddPane { get; set; }

    public SinglePaneConfig Config { get; set; } = null!;
    public Pane? Pane { get; set; }

    protected override void OnInitialized()
    {
        Config = ConfigJson.FromJson<SinglePaneConfig>() ?? new();

        if (Panes is not null)
            Pane = Panes.OrderBy(x => x.Ordinal).FirstOrDefault();
    }

    public String? GetConfigJson() => Config.ToJson();
}