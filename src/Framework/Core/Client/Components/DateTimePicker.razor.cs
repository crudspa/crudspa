namespace Crudspa.Framework.Core.Client.Components;

public partial class DateTimePicker
{
    [Parameter, EditorRequired] public DateTimeOffset? Value { get; set; }
    [Parameter] public String? Format { get; set; } = "g";
    [Parameter] public EventCallback<DateTimeOffset?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
}