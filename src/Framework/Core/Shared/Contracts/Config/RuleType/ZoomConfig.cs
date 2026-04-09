namespace Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

public class ZoomConfig : Observable
{
    public enum Levels
    {
        Smaller,
        Small,
        Regular,
        Large,
        Larger,
    }

    public Levels Level
    {
        get;
        set => SetProperty(ref field, value);
    }
}