namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class AccessCode : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Username
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? EmailAddress
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Code
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PortalName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Contact? Contact
    {
        get;
        set => SetProperty(ref field, value);
    }
}