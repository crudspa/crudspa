namespace Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

public class LineHeightConfig : Observable
{
    public String? LineHeight
    {
        get;
        set => SetProperty(ref field, value);
    }
}