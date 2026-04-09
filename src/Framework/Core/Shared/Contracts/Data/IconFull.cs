namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class IconFull : Observable, INamed
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

    public String? CssClass
    {
        get;
        set => SetProperty(ref field, value);
    }
}