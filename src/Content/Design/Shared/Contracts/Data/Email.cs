namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class Email : Observable, IValidates, INamed, ICountable
{
    public String? Name => Subject;

    public enum Statuses { Scheduled, Processing, Sent, Failed }

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

    public String? Subject
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FromName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FromEmail
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

    public String? Body
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

    public ObservableCollection<EmailAttachment> EmailAttachments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FromName.HasNothing())
                errors.AddError("From Name is required.", nameof(FromName));
            else if (FromName!.Length > 150)
                errors.AddError("From Name cannot be longer than 150 characters.", nameof(FromName));

            if (FromEmail.HasNothing())
                errors.AddError("From Email is required.", nameof(FromEmail));
            else if (FromEmail!.Length > 75)
                errors.AddError("From Email cannot be longer than 75 characters.", nameof(FromEmail));

            if (!Send.HasValue)
                errors.AddError("Send is required.", nameof(Send));

            if (Subject.HasNothing())
                errors.AddError("Subject is required.", nameof(Subject));
            else if (Subject!.Length > 150)
                errors.AddError("Subject cannot be longer than 150 characters.", nameof(Subject));

            if (Body.HasNothing())
                errors.AddError("Body is required.", nameof(Body));

            EmailAttachments.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}