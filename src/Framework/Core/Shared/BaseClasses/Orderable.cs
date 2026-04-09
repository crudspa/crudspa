namespace Crudspa.Framework.Core.Shared.BaseClasses;

public class Orderable : Named, IOrderable
{
    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }
}