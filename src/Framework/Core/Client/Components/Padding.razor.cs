namespace Crudspa.Framework.Core.Client.Components;

public partial class Padding
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public Sizes Size { get; set; } = Sizes.Tiny;
    [Parameter] public Directions Direction { get; set; } = Directions.All;

    public enum Sizes
    {
        Tight,
        Micro,
        Tiny,
        Small,
        Medium,
        Large,
    }

    public enum Directions
    {
        All,
        Horizontal,
        Vertical,
        Top,
        Right,
        Bottom,
        Left,
    }

    public String PaddingClass
    {
        get
        {
            var paddingClass = "c-padding";

            switch (Size)
            {
                case Sizes.Tight:
                    paddingClass += "-tight";
                    break;
                case Sizes.Micro:
                    paddingClass += "-micro";
                    break;
                case Sizes.Tiny:
                    paddingClass += "-tiny";
                    break;
                case Sizes.Small:
                    paddingClass += "-small";
                    break;
                case Sizes.Medium:
                    paddingClass += "-medium";
                    break;
                case Sizes.Large:
                    paddingClass += "-large";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Size));
            }

            switch (Direction)
            {
                case Directions.All:
                    break;
                case Directions.Horizontal:
                    paddingClass += "-hor";
                    break;
                case Directions.Vertical:
                    paddingClass += "-vert";
                    break;
                case Directions.Top:
                    paddingClass += "-top";
                    break;
                case Directions.Right:
                    paddingClass += "-right";
                    break;
                case Directions.Bottom:
                    paddingClass += "-bottom";
                    break;
                case Directions.Left:
                    paddingClass += "-left";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Direction));
            }

            return paddingClass;
        }
    }
}