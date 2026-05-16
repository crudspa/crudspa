namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class SmsMessage : Observable, IValidates, INamed, ICountable
{
    public String? Name => Body;
    public enum Directions { Inbound, Outbound }
    public enum Statuses { Queued, Sending, Sent, Delivered, Received, Undelivered, Failed, Canceled }
    public enum Providers { Twilio, LocalFile, Mock }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MembershipId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SmsId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SmsChannelKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MemberId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Body
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Directions Direction
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Occurred
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FromNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ToNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Statuses Status
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ProviderMessageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Providers Provider
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ApiResponse
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactPhoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContactFirstName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContactLastName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContactName
    {
        get
        {
            var name = $"{ContactFirstName} {ContactLastName}".Trim();
            return name.HasSomething() ? name : null;
        }
    }

    public String? CounterpartyNumber => Direction == Directions.Inbound ? FromNumber : ToNumber;

    public String? ConversationName => ContactName.HasSomething() ? ContactName : CounterpartyNumber;

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Body.HasNothing())
                errors.AddError("Body is required.", nameof(Body));
        });
    }
}