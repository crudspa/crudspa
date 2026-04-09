namespace Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

public class ColorPairConfig : Observable
{
    public String? Background
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Foreground
    {
        get;
        set => SetProperty(ref field, value);
    }
}