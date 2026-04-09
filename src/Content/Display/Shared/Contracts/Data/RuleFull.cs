namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class RuleFull : Observable, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DefaultValue
    {
        get;
        set => SetProperty(ref field, value);
    }

    public RuleTypeFull? RuleType
    {
        get;
        set => SetProperty(ref field, value);
    }
}