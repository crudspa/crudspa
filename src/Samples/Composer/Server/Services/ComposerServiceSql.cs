namespace Crudspa.Samples.Composer.Server.Services;

using Composer = Shared.Contracts.Data.Composer;

public class ComposerServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : IComposerService
{
    private static readonly Guid PortalId = new("aea2c861-459a-490c-b7c3-30e5156fec9f");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<Composer?>> Fetch(Request request)
    {
        return await wrappers.Try<Composer?>(request, async response =>
        {
            var composer = await ComposerSelect.Execute(Connection, request.SessionId);

            composer?.Organization = await organizationRepository.Select(Connection, composer.OrganizationId, PortalId) ?? new();

            return composer;
        });
    }

    public async Task<Response> Save(Request<Composer> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var composer = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, composer.Organization, PortalId));

            if (response.Errors.HasItems())
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await organizationRepository.Update(connection, transaction, request.SessionId, composer.Organization, PortalId);
                await ComposerUpdate.Execute(connection, transaction, request.SessionId, composer);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }
}