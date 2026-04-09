namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class TokenValue : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TokenId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Value
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TokenKey
    {
        get;
        set => SetProperty(ref field, value);
    }
}