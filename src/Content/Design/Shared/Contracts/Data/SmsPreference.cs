namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class SmsPreference : Observable, IValidates, INamed, ICountable
{
    public String? Name => Number;
    public enum Statuses { Unknown, OptedIn, OptedOut, Blocked }
    public enum Sources { System, Staff, InboundKeyword, Import, Provider }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SmsChannelKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Number
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

    public Guid? ContactPhoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContactPhonePhone
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Statuses Status
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Sources Source
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? StatusChanged
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Notes
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
            if (Number.HasNothing())
                errors.AddError("Number is required.", nameof(Number));
            else if (Number!.Length > 20)
                errors.AddError("Number cannot be longer than 20 characters.", nameof(Number));

            if (!StatusChanged.HasValue)
                errors.AddError("Status Changed is required.", nameof(StatusChanged));
        });
    }
}