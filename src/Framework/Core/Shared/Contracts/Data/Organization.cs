namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Organization : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String Name
    {
        get;
        set => SetProperty(ref field, value);
    } = "Organization";

    public String? TimeZoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Role> Roles
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 75)
                errors.AddError("Name cannot be longer than 75 characters.", nameof(Name));

            if (TimeZoneId.HasNothing())
                errors.AddError("Time Zone is required.", nameof(TimeZoneId));
            else if (TimeZoneId!.Length > 32)
                errors.AddError("Time Zone cannot be longer than 32 characters.", nameof(TimeZoneId));
        });
    }
}