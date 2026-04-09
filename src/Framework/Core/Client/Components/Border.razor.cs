namespace Crudspa.Framework.Core.Client.Components;

public partial class Border
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public Directions Direction { get; set; } = Directions.All;

    public enum Directions
    {
        All,
        Horizontal,
        Vertical,
        Top,
        Right,
        Bottom,
        Left,
        None,
    }

    public String BorderClass
    {
        get
        {
            const String borderClass = "c-border";

            switch (Direction)
            {
                case Directions.All:
                    return borderClass;
                case Directions.Horizontal:
                    return borderClass + "-hor";
                case Directions.Vertical:
                    return borderClass + "-vert";
                case Directions.Top:
                    return borderClass + "-top";
                case Directions.Right:
                    return borderClass + "-right";
                case Directions.Bottom:
                    return borderClass + "-bottom";
                case Directions.Left:
                    return borderClass + "-left";
                case Directions.None:
                    return String.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Direction));
            }
        }
    }
}