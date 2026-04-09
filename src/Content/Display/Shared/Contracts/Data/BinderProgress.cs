namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class BinderProgress : Observable
{
    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
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