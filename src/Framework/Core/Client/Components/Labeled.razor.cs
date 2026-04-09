namespace Crudspa.Framework.Core.Client.Components;

public partial class Labeled
{
    [Parameter, EditorRequired] public String Label { get; set; } = null!;
    [Parameter] public String CssClass { get; set; } = String.Empty;
    [Parameter] public String? ValueClass { get; set; } = String.Empty;
    [Parameter] public Boolean? BooleanValue { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public DateTimeOffset? DateTimeOffsetValue { get; set; }
    [Parameter] public DateOnly? DateOnlyValue { get; set; }
    [Parameter] public TimeOnly? TimeOnlyValue { get; set; }
    [Parameter] public Decimal? DecimalValue { get; set; }
    [Parameter] public Int32? IntegerValue { get; set; }
    [Parameter] public Single? SingleValue { get; set; }
    [Parameter] public String? StringValue { get; set; }
    [Parameter] public String? StringUrl { get; set; }
    [Parameter] public String DateFormat { get; set; } = "G";
    [Parameter] public String TimeFormat { get; set; } = "t";
    [Parameter] public String NumberFormat { get; set; } = "G";
    [Parameter] public String? ValuePrefix { get; set; } = String.Empty;
    [Parameter] public String? ValueSuffix { get; set; } = String.Empty;
    [Parameter] public String Width { get; set; } = "8em";
    [Parameter] public Boolean ExpectNoSpaces { get; set; }

    [Inject] public ISessionState SessionState { get; set; } = null!;

    public String ComputedValueClass => ValueClass ?? String.Empty;

    public String? DateTimeString
    {
        get
        {
            if (!DateTimeOffsetValue.HasValue)
                return null;

            var localTime = DateTimeOffsetValue.ToLocalTime(SessionState.TimeZoneId);

            return localTime?.ToString(DateFormat);
        }
    }
}