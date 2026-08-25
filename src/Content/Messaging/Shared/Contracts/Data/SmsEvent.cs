namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class SmsEvent : Observable, IValidates, INamed, ICountable
{
    public String? Name => ProviderMessageId;
    public enum Providers { Twilio, LocalFile, Mock }
    public enum Types { InboundMessage, StatusCallback, MediaDownload, Unknown }
    public enum Statuses { Received, Processed, Duplicate, Failed, Ignored }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ProviderMessageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SmsChannelKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Providers Provider
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Types Type
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ProviderStatus
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? SignatureValid
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public DateTimeOffset? Received
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Processed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Statuses Status
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ErrorMessage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SmsMessageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
        });
    }
}