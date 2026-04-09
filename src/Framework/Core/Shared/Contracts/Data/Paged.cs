namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Paged : Observable
{
    public Int32 PageSize
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 PageNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContinuationToken
    {
        get;
        set => SetProperty(ref field, value);
    }
}