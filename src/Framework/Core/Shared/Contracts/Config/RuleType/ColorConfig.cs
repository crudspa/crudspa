namespace Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

public class ColorConfig : Observable
{
    public String? Color
    {
        get;
        set => SetProperty(ref field, value);
    }
}