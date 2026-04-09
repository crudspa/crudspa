namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class User : Observable, IValidates, ICountable, INamed
{
    public String Name => Contact.Name;

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

    public Guid? OrganizationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Username
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? ResetPassword
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public String? OrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Contact Contact
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? MaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public ObservableCollection<Selectable> Roles
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Byte[]? PasswordHash
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Byte[]? PasswordSalt
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (MaySignIn == false)
                return;

            if (Username.HasNothing())
                errors.AddError("Username is required.", nameof(Username));
            else if (Username!.Length > 150)
                errors.AddError("Username cannot be longer than 150 characters.", nameof(Username));
        });
    }
}