using Crudspa.Framework.Auth.Server.Contracts.Behavior;
using Crudspa.Framework.Auth.Server.Sproxies;
using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Auth.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Events;
using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Repositories;

public class AuthConfigRepositorySql(IGatewayService gatewayService) : IAuthConfigRepository
{
    public async Task<AuthConfig> Select(String connection, Guid? organizationId) => new()
    {
        Connections = (await AuthConnectionSelectForOrganization.Execute(connection, organizationId)).ToObservable(),
        Policies = (await AuthPolicySelectForOrganization.Execute(connection, organizationId)).ToObservable(),
    };

    public async Task<IList<Guid>> Save(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid? sessionId,
        Guid? organizationId,
        AuthConfig config)
    {
        var existingConnections = await AuthConnectionSelectForOrganization.Execute(connection, transaction, organizationId);
        var existingPolicies = await AuthPolicySelectForOrganization.Execute(connection, transaction, organizationId);

        Prepare(config, organizationId);

        var changedConnections = existingConnections
            .Where(existing => config.Connections.All(x => x.Id != existing.Id)
                || !Same(existing, config.Connections.First(x => x.Id == existing.Id)))
            .Select(x => x.Id)
            .ToHashSet();

        var affectedPolicyIds = existingPolicies
            .Where(existing => changedConnections.Contains(existing.AuthConnectionId)
                || config.Policies.All(x => x.Id != existing.Id)
                || !SecuritySame(existing, config.Policies.First(x => x.Id == existing.Id)))
            .Select(x => x.Id)
            .OfType<Guid>()
            .Distinct()
            .ToList();

        foreach (var incoming in config.Connections)
        {
            var existing = existingConnections.FirstOrDefault(x => x.Id == incoming.Id);

            if (existing is null)
                await AuthConnectionInsert.Execute(connection, transaction, sessionId, incoming);
            else if (!Same(existing, incoming))
                await AuthConnectionUpdate.Execute(connection, transaction, sessionId, incoming);
        }

        foreach (var incoming in config.Policies)
        {
            var existing = existingPolicies.FirstOrDefault(x => x.Id == incoming.Id);

            if (existing is null)
                await AuthPolicyInsert.Execute(connection, transaction, sessionId, incoming);
            else if (!Same(existing, incoming))
                await AuthPolicyUpdate.Execute(connection, transaction, sessionId, incoming);
        }

        foreach (var existing in existingPolicies.Where(existing => config.Policies.All(x => x.Id != existing.Id)))
            await AuthPolicyDelete.Execute(connection, transaction, sessionId, existing);

        foreach (var existing in existingConnections.Where(existing => config.Connections.All(x => x.Id != existing.Id)))
            await AuthConnectionDelete.Execute(connection, transaction, sessionId, existing);

        if (affectedPolicyIds.HasItems())
            await AuthPolicySessionsRevoke.Execute(connection, transaction, affectedPolicyIds.Select(x => (Guid?)x), "policy-changed");

        return affectedPolicyIds;
    }

    public async Task PublishRevocations(IList<Guid> policyIds)
    {
        if (policyIds.HasItems())
            await gatewayService.Publish(new AuthSessionsRevoked { PolicyIds = policyIds });
    }

    private static void Prepare(AuthConfig config, Guid? organizationId)
    {
        foreach (var connection in config.Connections)
        {
            connection.Id ??= Guid.NewGuid();
            connection.OrganizationId = organizationId;
            connection.Enabled = config.Policies.Any(x => x.AuthConnectionId == connection.Id && x.Enabled);

            if (!AuthProviders.IsExternal(connection.Provider))
                connection.Tenant = null;
            else
                connection.Tenant = connection.Tenant?.Trim();
        }

        foreach (var policy in config.Policies)
        {
            policy.Id ??= Guid.NewGuid();
            policy.OrganizationId = organizationId;
            policy.Key = policy.Audience == AuthAudiences.Student ? policy.Key?.Trim().ToLowerInvariant() : null;
        }
    }

    private static Boolean Same(AuthConnection left, AuthConnection right) =>
        left.OrganizationId == right.OrganizationId
        && left.Provider == right.Provider
        && left.Tenant == right.Tenant
        && left.Enabled == right.Enabled;

    private static Boolean Same(AuthPolicy left, AuthPolicy right) =>
        SecuritySame(left, right) && left.Key == right.Key;

    private static Boolean SecuritySame(AuthPolicy left, AuthPolicy right) =>
        left.OrganizationId == right.OrganizationId
        && left.AuthConnectionId == right.AuthConnectionId
        && left.Audience == right.Audience
        && left.IdleTimeoutMinutes == right.IdleTimeoutMinutes
        && left.AbsoluteTimeoutMinutes == right.AbsoluteTimeoutMinutes
        && left.Persist == right.Persist
        && left.AutoRedirect == right.AutoRedirect
        && left.Fallback == right.Fallback
        && left.Enabled == right.Enabled;
}