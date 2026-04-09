namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class BinderTypeFull : Observable
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

    public String? DesignView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }
}