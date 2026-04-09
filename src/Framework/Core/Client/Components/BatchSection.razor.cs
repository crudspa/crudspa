namespace Crudspa.Framework.Core.Client.Components;

public partial class BatchSection
{
    [Parameter, EditorRequired] public String? Title { get; set; }
    [Parameter] public RenderFragment? HeadingPanel { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
}