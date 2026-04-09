namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Blob : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Byte[]? Data
    {
        get;
        set => SetProperty(ref field, value);
    }
}