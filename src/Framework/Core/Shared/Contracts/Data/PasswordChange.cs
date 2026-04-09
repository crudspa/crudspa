namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class PasswordChange : Observable, IValidates
{
    public String? NewPassword
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Confirm
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (NewPassword.HasNothing())
                errors.AddError("New Password is required.", nameof(NewPassword));
            else if (NewPassword.Length < 12)
                errors.AddError("Password must be at least 12 characters long.", nameof(NewPassword));

            if (Confirm.HasNothing())
                errors.AddError("Confirm is required.", nameof(Confirm));

            if (!errors.HasItems() && !NewPassword!.IsExactly(Confirm))
                errors.AddError("The values for New Password and Confirm do not match.", nameof(Confirm));
        });
    }
}