namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class EmailSent : Observable, INamed, ICountable
{
    public String? Name => RecipientName ?? RecipientEmail;

    public enum Statuses { Failed, Succeeded }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? EmailId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? RecipientId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RecipientEmail
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

    public String? ApiResponse
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RecipientFirstName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RecipientLastName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RecipientName
    {
        get
        {
            var name = $"{RecipientFirstName} {RecipientLastName}".Trim();
            return name.HasSomething() ? name : null;
        }
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }
}