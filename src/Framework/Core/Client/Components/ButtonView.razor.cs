namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonView
{
    [Parameter, EditorRequired] public String Href { get; set; } = null!;
    [Parameter] public String Text { get; set; } = "View";
}