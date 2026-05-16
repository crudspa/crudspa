namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class SmsOutboundMedia : Observable
{
    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContentType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Url
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