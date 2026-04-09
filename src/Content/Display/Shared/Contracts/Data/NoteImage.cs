namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class NoteImage : Observable, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NoteId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ImageFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? Url
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal

    {
        get;
        set => SetProperty(ref field, value);
    }
}