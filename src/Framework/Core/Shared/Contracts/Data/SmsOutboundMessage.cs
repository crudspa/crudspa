namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class SmsOutboundMessage : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SmsChannelKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? From
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? To
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Body
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusCallbackUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ProviderMessageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<SmsOutboundMedia> Media
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}