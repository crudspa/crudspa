namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Sort : Observable
{
    public String? Field
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Ascending
    {
        get;
        set => SetProperty(ref field, value);
    }
}