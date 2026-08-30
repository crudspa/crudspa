using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.BaseClasses;
using Crudspa.Framework.Core.Shared.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Extensions;

namespace Crudspa.Framework.Auth.Shared.Contracts.Data;

public class AuthConnection : Observable, IUnique, IValidates
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

    public String? Provider
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Tenant
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
            if (!AuthProviders.Names.ContainsKey(Provider ?? String.Empty))
                errors.AddError("Sign-in Method is required.", nameof(Provider));

            if (AuthProviders.IsExternal(Provider) && Tenant.HasNothing())
                errors.AddError($"{AuthProviders.Names.GetValueOrDefault(Provider!)} District ID is required.", nameof(Tenant));

            if (Tenant?.Length > 255)
                errors.AddError("District ID cannot be longer than 255 characters.", nameof(Tenant));
        });
    }
}