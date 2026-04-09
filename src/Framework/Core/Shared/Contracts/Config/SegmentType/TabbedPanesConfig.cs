namespace Crudspa.Framework.Core.Shared.Contracts.Config.SegmentType;

public class TabbedPanesConfig : Observable
{
    public enum Orientations { Horizontal, Vertical }

    public Orientations Orientation
    {
        get;
        set => SetProperty(ref field, value);
    }
}