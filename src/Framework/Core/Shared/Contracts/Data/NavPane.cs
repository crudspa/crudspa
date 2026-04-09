namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class NavPane : Observable, IOrderable, INamed
{
    public String? Name => Title;

    public Guid? Id
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

    public Guid? SegmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeDisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Lazy
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Boolean Loaded
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean IsNew
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ConfigJson
    {
        get;
        set => SetProperty(ref field, value);
    }
}