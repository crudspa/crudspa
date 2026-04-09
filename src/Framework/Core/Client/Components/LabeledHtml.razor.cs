namespace Crudspa.Framework.Core.Client.Components;

public partial class LabeledHtml
{
    [Parameter, EditorRequired] public String Label { get; set; } = null!;
    [Parameter] public String? HtmlValue { get; set; }
    [Parameter] public Boolean? HideLabel { get; set; } = false;
}