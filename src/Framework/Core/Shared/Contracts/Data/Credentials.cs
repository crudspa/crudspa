namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Credentials : Observable, IValidates
{
    public String? Username
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Password
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Username.HasNothing())
                errors.AddError("Username is required.", nameof(Username));
            else if (Username!.Length > 75)
                errors.AddError("Username cannot be longer than 75 characters.", nameof(Username));

            if (Password.HasNothing())
                errors.AddError("Password is required.", nameof(Password));
        });
    }
}