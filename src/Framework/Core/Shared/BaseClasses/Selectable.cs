namespace Crudspa.Framework.Core.Shared.BaseClasses;

public class Selectable : Named
{
    public Guid? RootId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Selected
    {
        get;
        set => SetProperty(ref field, value);
    } = false;
}