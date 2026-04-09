namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class EmailAddress : Observable
{
    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Address
    {
        get;
        set => SetProperty(ref field, value);
    }
}