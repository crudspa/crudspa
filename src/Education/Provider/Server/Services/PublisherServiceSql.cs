namespace Crudspa.Education.Provider.Server.Services;

public class PublisherServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : IPublisherService
{
    private static readonly Guid PortalId = new("de5e3ff9-ecdd-47d4-b05c-c61cde8994d4");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Publisher>>> Search(Request<PublisherSearch> request)
    {
        return await wrappers.Try<IList<Publisher>>(request, async response =>
        {
            var publishers = await PublisherSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var organizations = await organizationRepository.SelectByIds(Connection, publishers.Select(x => x.OrganizationId), PortalId);

            foreach (var publisher in publishers)
                publisher.Organization = organizations.First(x => x.Id.Equals(publisher.OrganizationId));

            return publishers;
        });
    }

    public async Task<Response<Publisher?>> Fetch(Request<Publisher> request)
    {
        return await wrappers.Try<Publisher?>(request, async response =>
        {
            var publisher = await PublisherSelect.Execute(Connection, request.SessionId, request.Value);

            publisher?.Organization = await organizationRepository.Select(Connection, publisher.OrganizationId, PortalId) ?? new();

            return publisher;
        });
    }

    public async Task<Response<Publisher?>> Add(Request<Publisher> request)
    {
        return await wrappers.Validate<Publisher?, Publisher>(request, async response =>
        {
            var publisher = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, publisher.Organization, PortalId));

            if (response.Errors.HasItems())
                return null;

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                publisher.OrganizationId = await organizationRepository.Insert(connection, transaction, request.SessionId, publisher.Organization, PortalId);

                var id = await PublisherInsert.Execute(connection, transaction, request.SessionId, publisher);

                return new Publisher
                {
                    Id = id,
                };
            });
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

    public async Task<Response> Remove(Request<Publisher> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var publisher = request.Value;
            var existing = await PublisherSelect.Execute(Connection, request.SessionId, publisher);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await PublisherDelete.Execute(connection, transaction, request.SessionId, publisher);

                await organizationRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.OrganizationId });
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }
}