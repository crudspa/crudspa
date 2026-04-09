namespace Crudspa.Content.Jobs.Shared.Contracts.Data;

public class EmailLog : Observable
{
    public enum Statuses { Failed, Succeeded }

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
}