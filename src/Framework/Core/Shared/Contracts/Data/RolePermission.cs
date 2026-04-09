namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class RolePermission : Observable
{
    public Guid? RoleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PermissionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Selected
    {
        get;
        set => SetProperty(ref field, value);
    }
}