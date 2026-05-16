namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class SmsMessageMedia : Observable, IValidates, INamed, IOrderable
{
    public String? Name => FileName;
    public enum DownloadStatuses { Pending, Downloaded, Failed, Skipped }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SmsMessageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FileName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? ContentType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SizeBytes
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DownloadStatuses DownloadStatus
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
        });
    }
}