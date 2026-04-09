namespace Crudspa.Framework.Core.Client.Components;

public partial class Wrapped
{
    [Parameter] public Boolean NoWrap { get; set; }
    [Parameter] public Boolean Star { get; set; }
    [Parameter] public Alignments Alignment { get; set; } = Alignments.Default;
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;

    public enum Alignments
    {
        Default,
        Top,
        Center,
        Bottom,
        Right,
    }

    public String WrapClass
    {
        get
        {
            var wrapClass = NoWrap ? "c-nowrap " : "c-wrap ";

            switch (Alignment)
            {
                case Alignments.Default:
                    break;
                case Alignments.Top:
                    wrapClass += "top ";
                    break;
                case Alignments.Center:
                    wrapClass += "center ";
                    break;
                case Alignments.Bottom:
                    wrapClass += "bottom ";
                    break;
                case Alignments.Right:
                    wrapClass += "right ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Alignment));
            }

            if (Star)
                wrapClass += "star";

            return wrapClass;
        }
    }
}