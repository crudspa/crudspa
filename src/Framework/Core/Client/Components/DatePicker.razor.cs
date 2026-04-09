namespace Crudspa.Framework.Core.Client.Components;

public partial class DatePicker
{
    [Parameter, EditorRequired] public DateOnly? Value { get; set; }
    [Parameter, EditorRequired] public String? Format { get; set; }
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
}