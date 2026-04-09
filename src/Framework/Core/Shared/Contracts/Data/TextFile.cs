namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class TextFile : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }
}