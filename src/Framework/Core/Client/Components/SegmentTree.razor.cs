namespace Crudspa.Framework.Core.Client.Components;

public partial class SegmentTree
{
    [Parameter] public String Prompt { get; set; } = "Select the destination segment:";
    [Parameter] public ObservableCollection<Expandable> Items { get; set; } = [];
    [Parameter] public Guid? SelectedId { get; set; }
    [Parameter] public EventCallback<Guid?> SelectedIdChanged { get; set; }
}