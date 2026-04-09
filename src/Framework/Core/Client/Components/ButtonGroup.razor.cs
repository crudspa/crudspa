namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonGroup
{
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public Margins Margin { get; set; } = Margins.None;

    public enum Margins { None, TinyHor }

    public String MarginClass => Margin switch
    {
        Margins.None => String.Empty,
        Margins.TinyHor => "c-margin-tiny-hor",
        _ => throw new ArgumentOutOfRangeException(nameof(Margin)),
    };
}