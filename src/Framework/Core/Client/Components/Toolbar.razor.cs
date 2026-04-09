namespace Crudspa.Framework.Core.Client.Components;

public partial class Toolbar
{
    public enum Borders { TopBottom, Bottom, None }

    [Parameter] public Borders Border { get; set; } = Borders.TopBottom;
    [Parameter] public RenderFragment? Left { get; set; }
    [Parameter] public RenderFragment? Right { get; set; }

    public String BorderClass => Border switch
    {
        Borders.TopBottom => "top-bottom-border",
        Borders.Bottom => "bottom-border",
        Borders.None => String.Empty,
        _ => throw new ArgumentOutOfRangeException(nameof(Border)),
    };
}