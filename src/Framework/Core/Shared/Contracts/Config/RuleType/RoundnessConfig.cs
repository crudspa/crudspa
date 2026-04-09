namespace Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

public class RoundnessConfig : Observable
{
    public String? Radius
    {
        get;
        set => SetProperty(ref field, value);
    }
}