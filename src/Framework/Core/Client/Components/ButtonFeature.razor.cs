namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonFeature
{
    [Parameter, EditorRequired] public String Href { get; set; } = null!;
    [Parameter, EditorRequired] public String Text { get; set; } = null!;
    [Parameter] public String? IconClass { get; set; }
}