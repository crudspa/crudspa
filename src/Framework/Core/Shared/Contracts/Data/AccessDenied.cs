namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class AccessDenied : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SessionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? EventType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PermissionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Method
    {
        get;
        set => SetProperty(ref field, value);
    }
}