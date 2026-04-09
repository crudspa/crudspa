namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ElementSpec : Observable
{
    public ElementType ElementType
    {
        get;
        set => SetProperty(ref field, value);
    } = null!;

    public Guid? SectionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }
}