namespace Crudspa.Framework.Core.Client.Components;

public partial class NumericTextBox<T>
{
    [Parameter] public T Value { get; set; } = default!;
    [Parameter] public EventCallback<T> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public String? Format { get; set; } = "G";
    [Parameter] public String? Prefix { get; set; } = String.Empty;
    [Parameter] public String? Suffix { get; set; } = String.Empty;
}