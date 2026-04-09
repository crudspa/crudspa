namespace Crudspa.Framework.Core.Client.Components;

public partial class Field
{
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public RenderFragment? LabelActions { get; set; }
    [Parameter] public String? Label { get; set; }
    [Parameter] public Containers Container { get; set; } = Containers.Label;
    [Parameter] public Sizes Size { get; set; } = Sizes.Unspecified;
    [Parameter] public Orientations Orientation { get; set; } = Orientations.Vertical;
    [Parameter] public Paddings Padding { get; set; } = Paddings.Default;

    public enum Containers { Label, Div }

    public enum Sizes
    {
        Unspecified,
        Pico,
        Nano,
        Micro,
        Min,
        Tinier,
        Tiny,
        Small,
        MediumSmall,
        Medium,
        MediumLarge,
        Large,
        Larger,
        Wide,
        Max,
        Full,
        Star,
    }

    public enum Orientations { Vertical, Horizontal }

    public enum Paddings
    {
        Default,
        None,
        TightVert,
        TightHor,
        TightTop,
        TightRight,
        TightBottom,
        TightLeft,
    }

    public String SizeClass => Size switch
    {
        Sizes.Unspecified => String.Empty,
        Sizes.Pico => "pico",
        Sizes.Nano => "nano",
        Sizes.Micro => "micro",
        Sizes.Min => "min",
        Sizes.Tinier => "tinier",
        Sizes.Tiny => "tiny",
        Sizes.Small => "small",
        Sizes.MediumSmall => "medium-small",
        Sizes.Medium => "medium",
        Sizes.MediumLarge => "medium-large",
        Sizes.Large => "large",
        Sizes.Larger => "larger",
        Sizes.Wide => "wide",
        Sizes.Max => "max",
        Sizes.Full => "full",
        Sizes.Star => "star",
        _ => throw new ArgumentOutOfRangeException(nameof(Size)),
    };

    public String OrientationClass => Orientation switch
    {
        Orientations.Vertical => String.Empty,
        Orientations.Horizontal => "horizontal",
        _ => throw new ArgumentOutOfRangeException(nameof(Orientation)),
    };

    public String PaddingClass => Padding switch
    {
        Paddings.Default => String.Empty,
        Paddings.None => "no-margin",
        Paddings.TightVert => "tight-vert",
        Paddings.TightHor => "tight-hor",
        Paddings.TightTop => "tight-top",
        Paddings.TightRight => "tight-right",
        Paddings.TightBottom => "tight-bottom",
        Paddings.TightLeft => "tight-left",
        _ => throw new ArgumentOutOfRangeException(nameof(Padding)),
    };
}