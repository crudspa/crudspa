namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface ISegmentDesign : IConfigDesign
{
    public String? Path { get; set; }
    Guid? Id { get; set; }
    ObservableCollection<Pane>? Panes { get; set; }
    BatchModel<Pane>? PanesModel { get; set; }
    List<Named>? PermissionNames { get; set; }
    List<PaneTypeFull>? PaneTypes { get; set; }
    Action? AddPane { get; set; }
}