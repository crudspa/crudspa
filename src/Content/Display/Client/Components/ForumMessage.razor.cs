namespace Crudspa.Content.Display.Client.Components;

public partial class ForumMessage
{
    [Parameter, EditorRequired] public Comment Comment { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment Body { get; set; } = null!;
    [Parameter] public RenderFragment? Footer { get; set; }
    [Parameter] public String? Id { get; set; }
    [Parameter] public String? Title { get; set; }
    [Parameter] public Boolean Pinned { get; set; }
    [Parameter] public Boolean Opening { get; set; }
    [Parameter] public Boolean ShowFooter { get; set; } = true;
    [Parameter] public Int32 Depth { get; set; }

    private String? DepthStyle => Depth == 0 ? null : $"--forum-depth: {Depth}";
}