using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.BaseClasses;
using Crudspa.Framework.Core.Shared.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Extensions;

namespace Crudspa.Framework.Auth.Shared.Contracts.Data;

public class AuthPolicy : Observable, IUnique, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? OrganizationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AuthConnectionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Audience
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 IdleTimeoutMinutes
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 AbsoluteTimeoutMinutes
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Persist
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean AutoRedirect
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Fallback
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Enabled
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Audience.HasNothing()
                || Audience!.Length > 25
                || Audience.Any(x => !Char.IsAsciiLetterLower(x) && !Char.IsDigit(x) && x != '-'))
                errors.AddError("Sign-in Audience must be a lowercase portal key.", nameof(Audience));

            if (AuthConnectionId is null)
                errors.AddError("Sign-in Method is required.", nameof(AuthConnectionId));

            if (Audience == AuthAudiences.Student)
            {
                if (Key.HasNothing())
                    errors.AddError("District Link is required.", nameof(Key));
                else if (Key!.Length > 75 || Key.Any(x => !Char.IsAsciiLetterLower(x) && !Char.IsDigit(x) && x != '-'))
                    errors.AddError("District Link can contain only lowercase letters, numbers, and hyphens.", nameof(Key));
            }
            else if (Key.HasSomething())
                errors.AddError("District Link is only available for Student sign-in.", nameof(Key));

            if (IdleTimeoutMinutes <= 0)
                errors.AddError("Inactivity Limit must be greater than zero.", nameof(IdleTimeoutMinutes));

            if (AbsoluteTimeoutMinutes < IdleTimeoutMinutes)
                errors.AddError("Overall Limit cannot be shorter than the Inactivity Limit.", nameof(AbsoluteTimeoutMinutes));
        });
    }
}