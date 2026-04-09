namespace Crudspa.Framework.Core.Client.Components;

public partial class PaneLink
{
    [Parameter, EditorRequired] public String Href { get; set; } = null!;
    [Parameter, EditorRequired] public String Text { get; set; } = null!;
    [Parameter] public Int32? Count { get; set; }
}