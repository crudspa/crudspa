namespace Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

public class FontConfig : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Size
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Weight
    {
        get;
        set => SetProperty(ref field, value);
    }
}