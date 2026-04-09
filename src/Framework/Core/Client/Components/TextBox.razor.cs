namespace Crudspa.Framework.Core.Client.Components;

public partial class TextBox
{
    [Parameter] public String? Value { get; set; }
    [Parameter] public String? Placeholder { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Int32? MaxLength { get; set; }
}