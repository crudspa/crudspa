namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ElementLink : Observable
{
    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Url
    {
        get;
        set => SetProperty(ref field, value);
    }
}