namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class DistrictMember : Contact
{
    public Guid? UserId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? MaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UserOrganizationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? UserUsername
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? UserResetPassword
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public ObservableCollection<Selectable> Roles
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? AddressCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? DistrictId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DistrictName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            //errors.AddRange(base.Validate());

            if (FirstName.HasNothing())
                errors.AddError("First Name is required.", nameof(FirstName));
            else if (FirstName!.Length > 75)
                errors.AddError("First Name cannot be longer than 75 characters.", nameof(FirstName));

            if (LastName.HasNothing())
                errors.AddError("Last Name is required.", nameof(LastName));
            else if (LastName!.Length > 75)
                errors.AddError("Last Name cannot be longer than 75 characters.", nameof(LastName));

            if (TimeZoneId.HasNothing())
                errors.AddError("Time Zone is required.", nameof(TimeZoneId));
            else if (TimeZoneId!.Length > 32)
                errors.AddError("Time Zone cannot be longer than 32 characters.", nameof(TimeZoneId));

            if (MaySignIn == true)
            {
                if (UserUsername.HasNothing())
                    errors.AddError("Username is required.", nameof(UserUsername));
                else if (UserUsername!.Length > 75)
                    errors.AddError("Username cannot be longer than 75 characters.", nameof(UserUsername));
            }
        });
    }
}