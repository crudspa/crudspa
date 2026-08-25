namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class Sms : Observable, IValidates, INamed, ICountable
{
    public String? Name => String.Join(" | ", new[] { MembershipName, Send?.ToString("g") }.Where(x => x.HasSomething()));
    public enum Statuses { Scheduled, Processing, Sent, Failed, Canceled }

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

    public String? MembershipName
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

    public String? Body
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TemplateId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TemplateTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Send
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Statuses Status
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Processed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<SmsAttachment> SmsAttachments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!Send.HasValue)
                errors.AddError("Send is required.", nameof(Send));

            if (Body.HasNothing())
                errors.AddError("Body is required.", nameof(Body));

            SmsAttachments.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}