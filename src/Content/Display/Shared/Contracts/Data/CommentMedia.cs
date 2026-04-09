namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class CommentMedia : Observable, IValidates, IOrderable
{
    public enum Types { Audio, Image, Pdf, Video }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CommentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Types Type
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile AudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public PdfFile PdfFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public VideoFile VideoFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}