namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class BinderForPane : Observable
{
    public Guid? PaneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Binder? Binder
    {
        get;
        set => SetProperty(ref field, value);
    }
}