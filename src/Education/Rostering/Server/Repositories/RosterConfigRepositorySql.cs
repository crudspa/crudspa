using Crudspa.Education.Rostering.Server.Contracts.Behavior;
using Crudspa.Education.Rostering.Server.Sproxies;
using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Education.Rostering.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.Extensions;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Repositories;

public class RosterConfigRepositorySql(ICryptographyService cryptographyService) : IRosterConfigRepository
{
    public async Task<RosterConfig> Select(String connection, Guid? organizationId)
    {
        var sources = await RosterSourceSelectForOrganization.Execute(connection, organizationId);
        foreach (var source in sources.Where(x => x.ClientSecret.HasSomething()))
            source.ClientSecret = cryptographyService.Unprotect(source.ClientSecret!);

        return new() { Sources = sources.ToObservable() };
    }

    public async Task Save(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid? sessionId,
        Guid? organizationId,
        Guid? deviceId,
        RosterConfig config)
    {
        var existingSources = await RosterSourceSelectForOrganization.Execute(connection, transaction, organizationId);
        var protectedSourceIds = existingSources
            .Where(x => x.ClientSecret?.StartsWith("v1:", StringComparison.Ordinal) == true)
            .Select(x => x.Id)
            .ToHashSet();

        foreach (var source in existingSources.Where(x => x.ClientSecret.HasSomething()))
            source.ClientSecret = cryptographyService.Unprotect(source.ClientSecret!);

        foreach (var source in config.Sources)
        {
            source.Id ??= Guid.NewGuid();
            source.OrganizationId = organizationId;

            if (!RosterProviders.UsesTenant(source.Provider))
                source.Tenant = null;
            else
                source.Tenant = source.Tenant?.Trim();

            if (source.Provider == RosterProviders.OneRoster)
            {
                source.ClientId = source.ClientId?.Trim();
                source.ClientSecret = source.ClientSecret?.Trim();
                source.TokenUrl = source.TokenUrl?.Trim();
                source.BaseUrl = source.BaseUrl?.Trim().TrimEnd('/');
            }
            else
                source.ClientId = source.ClientSecret = source.TokenUrl = source.BaseUrl = null;

            var existing = existingSources.FirstOrDefault(x => x.Id == source.Id);
            var existingScheduleId = existing?.ScheduleId;

            source.ScheduleId = source.Recurring
                ? await RosterScheduleSave.Execute(connection, transaction, sessionId, deviceId, source)
                : null;

            var protectedSecret = source.ClientSecret.HasSomething() ? cryptographyService.Protect(source.ClientSecret!) : source.ClientSecret;
            var needsProtection = existing is not null
                && source.ClientSecret != protectedSecret
                && !protectedSourceIds.Contains(existing.Id);
            if (existing is null || !Same(existing, source) || needsProtection)
            {
                var secret = source.ClientSecret;
                source.ClientSecret = protectedSecret;
                if (existing is null)
                    await RosterSourceInsert.Execute(connection, transaction, sessionId, source);
                else
                    await RosterSourceUpdate.Execute(connection, transaction, sessionId, source);
                source.ClientSecret = secret;
            }

            if (!source.Recurring && existingScheduleId is not null)
                await RosterScheduleDelete.Execute(connection, transaction, sessionId, existingScheduleId);
        }

        foreach (var existing in existingSources.Where(existing => config.Sources.All(x => x.Id != existing.Id)))
        {
            await RosterSourceDelete.Execute(connection, transaction, sessionId, existing);

            if (existing.ScheduleId is not null)
                await RosterScheduleDelete.Execute(connection, transaction, sessionId, existing.ScheduleId);
        }
    }

    private static Boolean Same(RosterSource left, RosterSource right) =>
        left.OrganizationId == right.OrganizationId
        && left.Provider == right.Provider
        && left.Tenant == right.Tenant
        && left.ClientId == right.ClientId
        && left.ClientSecret == right.ClientSecret
        && left.TokenUrl == right.TokenUrl
        && left.BaseUrl == right.BaseUrl
        && left.Mode == right.Mode
        && left.ScheduleId == right.ScheduleId
        && left.Recurring == right.Recurring
        && left.ScheduleHour == right.ScheduleHour
        && left.ScheduleMinute == right.ScheduleMinute
        && left.ScheduleTimeZoneId == right.ScheduleTimeZoneId;
}