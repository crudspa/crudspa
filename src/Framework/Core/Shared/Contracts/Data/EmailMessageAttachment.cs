namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class EmailMessageAttachment : Observable
{
    public String? Name
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