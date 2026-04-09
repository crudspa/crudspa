namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class MediaPlay : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AudioFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? VideoFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Started
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Canceled
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Completed
    {
        get;
        set => SetProperty(ref field, value);
    }
}