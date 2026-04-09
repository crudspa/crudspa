namespace Crudspa.Education.Publisher.Server.Services;

using Publisher = Shared.Contracts.Data.Publisher;

public class PublisherServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : IPublisherService
{
    private static readonly Guid PortalId = new("de5e3ff9-ecdd-47d4-b05c-c61cde8994d4");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<Publisher?>> Fetch(Request request)
    {
        return await wrappers.Try<Publisher?>(request, async response =>
        {
            var publisher = await PublisherSelect.Execute(Connection, request.SessionId);

            publisher?.Organization = await organizationRepository.Select(Connection, publisher.OrganizationId, PortalId) ?? new();

            return publisher;
        });
    }

    public async Task<Response> Save(Request<Publisher> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var publisher = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, publisher.Organization, PortalId));

            if (response.Errors.HasItems())
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await organizationRepository.Update(connection, transaction, request.SessionId, publisher.Organization, PortalId);
                await PublisherUpdate.Execute(connection, transaction, request.SessionId, publisher);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }
}