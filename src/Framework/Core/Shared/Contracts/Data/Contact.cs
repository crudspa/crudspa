namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Contact : Observable, IValidates, ICountable, INamed
{
    public String Name => String.Join(" ", new[] { FirstName?.Trim(), LastName?.Trim() }.Where(x => x.HasSomething()));

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FirstName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? LastName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TimeZoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ContactEmail> Emails
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ContactPhone> Phones
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ContactPostal> Postals
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Dictionary<String, String> TokenValues
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String? Email
    {
        get;
        set => SetProperty(ref field, value);
    }

    public virtual List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FirstName.HasNothing() && LastName.HasNothing())
                errors.AddError("First name or last name is required.");

            if (FirstName.HasSomething() && FirstName.Length > 75)
                errors.AddError("First Name cannot be longer than 75 characters.", nameof(FirstName));

            if (LastName.HasSomething() && LastName.Length > 75)
                errors.AddError("Last Name cannot be longer than 75 characters.", nameof(LastName));

            if (TimeZoneId.HasNothing())
                errors.AddError("Time Zone Id is required.", nameof(TimeZoneId));
            else if (TimeZoneId!.Length > 32)
                errors.AddError("Time Zone Id cannot be longer than 32 characters.", nameof(TimeZoneId));
        });
    }
}