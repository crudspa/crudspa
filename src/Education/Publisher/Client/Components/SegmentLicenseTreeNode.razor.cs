namespace Crudspa.Education.Publisher.Client.Components;

public partial class SegmentLicenseTreeNode
{
    [Parameter, EditorRequired] public Expandable Node { get; set; } = null!;
    [Parameter] public Int32 Depth { get; set; }
    [Parameter, EditorRequired] public Func<Expandable, Boolean> Visible { get; set; } = null!;
    [Parameter, EditorRequired] public Func<Expandable, Boolean> Indeterminate { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback<Expandable> SelectionChanged { get; set; }
    [Parameter, EditorRequired] public EventCallback<Expandable> IncludeBranch { get; set; }
    [Parameter, EditorRequired] public EventCallback<Expandable> ExcludeBranch { get; set; }

    private void ToggleExpanded()
    {
        if (Node.Children.HasItems())
            Node.Expanded = Node.Expanded != true;
    }

    private async Task HandleSelectionChanged(ChangeEventArgs args)
    {
        Node.Selected = args.Value as Boolean? == true;
        await SelectionChanged.InvokeAsync(Node);
    }
}