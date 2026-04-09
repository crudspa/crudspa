namespace Crudspa.Education.Provider.Server.Services;

using Provider = Shared.Contracts.Data.Provider;

public class ProviderServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : IProviderService
{
    private static readonly Guid PortalId = new("2f6b54e3-689f-46a3-a1ee-30a4bfe18d63");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<Provider?>> Fetch(Request request)
    {
        return await wrappers.Try<Provider?>(request, async response =>
        {
            var provider = await ProviderSelect.Execute(Connection, request.SessionId);

            provider?.Organization = await organizationRepository.Select(Connection, provider.OrganizationId, PortalId) ?? new();

            return provider;
        });
    }

    public async Task<Response> Save(Request<Provider> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var provider = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, provider.Organization, PortalId));

            if (response.Errors.HasItems())
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await organizationRepository.Update(connection, transaction, request.SessionId, provider.Organization, PortalId);
                await ProviderUpdate.Execute(connection, transaction, request.SessionId, provider);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }
}