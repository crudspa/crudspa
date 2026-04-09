namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class EmailMessage : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public EmailAddress? From
    {
        get;
        set => SetProperty(ref field, value);
    }

    public EmailAddress? ReplyTo
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<EmailAddress> To
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<EmailAddress> Cc
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<EmailAddress> Bcc
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String? Subject
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Message
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<EmailMessageAttachment> Attachments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Boolean Private
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Sent
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MessageId
    {
        get;
        set => SetProperty(ref field, value);
    }
}