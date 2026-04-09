namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Notepage : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NotebookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NoteId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SelectedImageFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public NoteElement? Note
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile? SelectedImageFile
    {
        get;
        set => SetProperty(ref field, value);
    }
}