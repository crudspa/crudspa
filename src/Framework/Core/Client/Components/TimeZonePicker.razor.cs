namespace Crudspa.Framework.Core.Client.Components;

public partial class TimeZonePicker
{
    [Parameter] public String? Value { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean AllowNull { get; set; }
    [Parameter] public String? NullText { get; set; } = "[None]";

    private IReadOnlyList<TimeZoneInfo> TimeZones { get; } = TimeZoneInfo.GetSystemTimeZones();
}