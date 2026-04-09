namespace Crudspa.Samples.Catalog.Server.Services;

using Catalog = Shared.Contracts.Data.Catalog;

public class CatalogServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : ICatalogService
{
    private static readonly Guid PortalId = new("651a367c-a7dd-4fe8-be5a-b70ef275a8ec");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<Catalog?>> Fetch(Request request)
    {
        return await wrappers.Try<Catalog?>(request, async response =>
        {
            var catalog = await CatalogSelect.Execute(Connection, request.SessionId);

            catalog?.Organization = await organizationRepository.Select(Connection, catalog.OrganizationId, PortalId) ?? new();

            return catalog;
        });
    }

    public async Task<Response> Save(Request<Catalog> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var catalog = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, catalog.Organization, PortalId));

            if (response.Errors.HasItems())
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await organizationRepository.Update(connection, transaction, request.SessionId, catalog.Organization, PortalId);
                await CatalogUpdate.Execute(connection, transaction, request.SessionId, catalog);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }
}