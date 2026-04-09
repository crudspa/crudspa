namespace Crudspa.Education.Common.Client.Models;

public class CatchModel : ChoicesModel
{
    public enum Edges { Top, Right, Bottom, Left }

    private Timer? _bounceTimer;

    public Boolean Bouncing
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void BounceAround()
    {
        foreach (var option in Options)
            option.PositionStyle = GetRandomEdgePosition();

        if (!Bouncing)
        {
            Bouncing = true;
            _bounceTimer = new(_ => BounceAround(), null, 10, Timeout.Infinite);
        }
        else
            _bounceTimer?.Change(3000, Timeout.Infinite);

        RaisePropertyChanged(nameof(Options));
    }

    public void StopBouncing()
    {
        _bounceTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _bounceTimer = null;

        Bouncing = false;

        foreach (var option in Options)
            option.PositionStyle = String.Empty;

        RaisePropertyChanged(nameof(Options));
    }

    public static String GetRandomEdgePosition()
    {
        const String styles = "position: fixed; transition: left 3s ease-in-out, top 3s ease-in-out; z-index: 600; ";

        var random = new Random();
        var edge = (Edges)random.Next(0, 4);
        var position = random.Next(2, 98);

        switch (edge)
        {
            case Edges.Left:
                return styles + $"top: calc({position}vh - 4em); left: 0;";
            case Edges.Top:
                return styles + $"top: 4em; left: calc({position}vw - 6em);";
            case Edges.Right:
                return styles + $"top: calc({position}vh - 4em); left: calc(100vw - 12em);";
            case Edges.Bottom:
            default:
                return styles + $"top: calc(100vh - 12em); left: calc({position}vw - 6em);";
        }
    }
}