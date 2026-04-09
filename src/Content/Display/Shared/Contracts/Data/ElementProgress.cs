namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ElementProgress : Observable
{
    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TimesCompleted
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;
}