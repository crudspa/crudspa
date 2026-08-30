using Crudspa.Education.Rostering.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.BaseClasses;
using Crudspa.Framework.Core.Shared.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Extensions;

namespace Crudspa.Education.Rostering.Shared.Contracts.Data;

public class RosterSource : Observable, IUnique, IValidates
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

    public String? ClientId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ClientSecret
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TokenUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BaseUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Mode
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ScheduleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Checkpoint
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? LastSucceeded
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Recurring
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ScheduleHour
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ScheduleMinute
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ScheduleTimeZoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!RosterProviders.Names.ContainsKey(Provider ?? String.Empty))
                errors.AddError("Roster Provider is required.", nameof(Provider));

            if (!RosterModes.Names.ContainsKey(Mode ?? String.Empty))
                errors.AddError("Roster Mode is required.", nameof(Mode));

            if (RosterProviders.UsesTenant(Provider) && Tenant.HasNothing())
                errors.AddError($"{RosterProviders.Names.GetValueOrDefault(Provider!)} District ID is required.", nameof(Tenant));

            if (Tenant?.Length > 255)
                errors.AddError("District ID cannot be longer than 255 characters.", nameof(Tenant));

            if (Provider == RosterProviders.OneRoster)
            {
                if (ClientId.HasNothing())
                    errors.AddError("Client ID is required.", nameof(ClientId));
                if (ClientSecret.HasNothing())
                    errors.AddError("Client Secret is required.", nameof(ClientSecret));
                if (!Https(TokenUrl))
                    errors.AddError("Token URL must be a valid HTTPS URL.", nameof(TokenUrl));
                if (!Https(BaseUrl))
                    errors.AddError("Base URL must be a valid HTTPS URL.", nameof(BaseUrl));
            }

            if (ClientId?.Length > 255)
                errors.AddError("Client ID cannot be longer than 255 characters.", nameof(ClientId));
            if (ClientSecret?.Length > 500)
                errors.AddError("Client Secret cannot be longer than 500 characters.", nameof(ClientSecret));
            if (TokenUrl?.Length > 500)
                errors.AddError("Token URL cannot be longer than 500 characters.", nameof(TokenUrl));
            if (BaseUrl?.Length > 500)
                errors.AddError("Base URL cannot be longer than 500 characters.", nameof(BaseUrl));

            if (Recurring)
            {
                if (Mode == RosterModes.Disabled)
                    errors.AddError("Recurring sync requires an enabled roster source.", nameof(Recurring));

                if (ScheduleHour is < 0 or > 23)
                    errors.AddError("Sync hour must be between 0 and 23.", nameof(ScheduleHour));

                if (ScheduleMinute is < 0 or > 59)
                    errors.AddError("Sync minute must be between 0 and 59.", nameof(ScheduleMinute));

                if (ScheduleTimeZoneId.HasNothing())
                    errors.AddError("Sync time zone is required.", nameof(ScheduleTimeZoneId));
                else if (ScheduleTimeZoneId!.Length > 32)
                    errors.AddError("Sync time zone cannot be longer than 32 characters.", nameof(ScheduleTimeZoneId));
                else
                {
                    try
                    {
                        TimeZoneInfo.FindSystemTimeZoneById(ScheduleTimeZoneId);
                    }
                    catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
                    {
                        errors.AddError("Sync time zone is invalid.", nameof(ScheduleTimeZoneId));
                    }
                }
            }
        });
    }

    private static Boolean Https(String? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
}