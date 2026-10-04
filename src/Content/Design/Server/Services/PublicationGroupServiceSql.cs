
namespace Crudspa.Content.Design.Server.Services;

public class PublicationGroupServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IPublicationGroupService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<PublicationGroup>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<PublicationGroup>>(request, async response =>
        {
            var publicationGroups = await PublicationGroupSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return publicationGroups;
        });
    }

    public async Task<Response<PublicationGroup?>> Fetch(Request<PublicationGroup> request)
    {
        return await wrappers.Try<PublicationGroup?>(request, async response =>
        {
            var publicationGroup = await PublicationGroupSelect.Execute(Connection, request.SessionId, request.Value);

            return publicationGroup;
        });
    }

    public async Task<Response<PublicationGroup?>> Add(Request<PublicationGroup> request)
    {
        return await wrappers.Validate<PublicationGroup?, PublicationGroup>(request, async response =>
        {
            var publicationGroup = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await PublicationGroupInsert.Execute(connection, transaction, request.SessionId, publicationGroup);

                return new PublicationGroup
                {
                    Id = id,
                    PortalId = publicationGroup.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<PublicationGroup> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var publicationGroup = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PublicationGroupUpdate.Execute(connection, transaction, request.SessionId, publicationGroup);
            });
        });
    }

    public async Task<Response> Remove(Request<PublicationGroup> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var publicationGroup = request.Value;
            var existing = await PublicationGroupSelect.Execute(Connection, request.SessionId, publicationGroup);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PublicationGroupDelete.Execute(connection, transaction, request.SessionId, publicationGroup);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<PublicationGroup>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var publicationGroups = request.Value;

            publicationGroups.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PublicationGroupUpdateOrdinals.Execute(connection, transaction, request.SessionId, publicationGroups);
            });
        });
    }
}