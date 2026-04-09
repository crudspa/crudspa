namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class PortalFeature : Observable, INamed
{
    public String? Name => Title;

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

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? IconId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PermissionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? IconCssClass
    {
        get;
        set => SetProperty(ref field, value);
    }
}