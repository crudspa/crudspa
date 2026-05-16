namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class BinderPane : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PaneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }
}