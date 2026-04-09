namespace Crudspa.Framework.Core.Client.Components;

public partial class Margin
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

    public String MarginClass
    {
        get
        {
            var marginClass = "c-margin";

            switch (Size)
            {
                case Sizes.Tight:
                    marginClass += "-tight";
                    break;
                case Sizes.Micro:
                    marginClass += "-micro";
                    break;
                case Sizes.Tiny:
                    marginClass += "-tiny";
                    break;
                case Sizes.Small:
                    marginClass += "-small";
                    break;
                case Sizes.Medium:
                    marginClass += "-medium";
                    break;
                case Sizes.Large:
                    marginClass += "-large";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Size));
            }

            switch (Direction)
            {
                case Directions.All:
                    break;
                case Directions.Horizontal:
                    marginClass += "-hor";
                    break;
                case Directions.Vertical:
                    marginClass += "-vert";
                    break;
                case Directions.Top:
                    marginClass += "-top";
                    break;
                case Directions.Right:
                    marginClass += "-right";
                    break;
                case Directions.Bottom:
                    marginClass += "-bottom";
                    break;
                case Directions.Left:
                    marginClass += "-left";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Direction));
            }

            return marginClass;
        }
    }
}