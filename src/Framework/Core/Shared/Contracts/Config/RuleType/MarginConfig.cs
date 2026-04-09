namespace Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

public class MarginConfig : Observable
{
    public String? Top
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Right
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Bottom
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Left
    {
        get;
        set => SetProperty(ref field, value);
    }
}