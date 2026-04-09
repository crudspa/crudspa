namespace Crudspa.Framework.Core.Client.Components;

public partial class MultilineTextBox
{
    [Parameter] public String? Value { get; set; }
    [Parameter] public String? Placeholder { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean Code { get; set; }
    [Parameter] public Boolean Spellcheck { get; set; } = true;
    [Parameter] public Heights Height { get; set; } = Heights.Unspecified;

    public enum Heights
    {
        Unspecified,
        Mini,
        Short,
        Average,
        Tall,
        TallX2,
        TallX3,
        TallX4,
    }

    public String HeightClass => Height switch
    {
        Heights.Unspecified => String.Empty,
        Heights.Mini => "mini",
        Heights.Short => "short",
        Heights.Average => "average",
        Heights.Tall => "tall",
        Heights.TallX2 => "tallx2",
        Heights.TallX3 => "tallx3",
        Heights.TallX4 => "tallx4",
        _ => throw new ArgumentOutOfRangeException(nameof(Height)),
    };
}