namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ContactEmail : Observable, IOrderable, IValidates
{
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

    public String? Email
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Email.HasNothing())
                errors.AddError("Email Address is required.", nameof(Email));
            else if (Email!.Length > 75)
                errors.AddError("Email Address cannot be longer than 75 characters.", nameof(Email));
            else if (!Email.IsEmailAddress())
                errors.AddError("Email Address must be properly formatted.", nameof(Email));
        });
    }
}