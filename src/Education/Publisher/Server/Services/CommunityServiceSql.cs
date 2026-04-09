namespace Crudspa.Education.Publisher.Server.Services;

public class CommunityServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ICommunityService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Community>>> FetchForDistrict(Request<District> request)
    {
        return await wrappers.Try<IList<Community>>(request, async response =>
        {
            var communities = await CommunitySelectForDistrict.Execute(Connection, request.SessionId, request.Value.Id);
            return communities;
        });
    }

    public async Task<Response<Community?>> Fetch(Request<Community> request)
    {
        return await wrappers.Try<Community?>(request, async response =>
        {
            var community = await CommunitySelect.Execute(Connection, request.SessionId, request.Value);
            return community;
        });
    }

    public async Task<Response<Community?>> Add(Request<Community> request)
    {
        return await wrappers.Validate<Community?, Community>(request, async response =>
        {
            var community = request.Value;

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await CommunityInsert.Execute(connection, transaction, request.SessionId, community);

                foreach (var communitySteward in community.CommunityStewards)
                {
                    communitySteward.CommunityId = id;
                    await CommunityStewardInsertByBatch.Execute(connection, transaction, request.SessionId, communitySteward);
                }

                return new Community
                {
                    Id = id,
                    DistrictId = community.DistrictId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Community> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var community = request.Value;

            var existing = await CommunitySelect.Execute(Connection, request.SessionId, community);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await CommunityUpdate.Execute(connection, transaction, request.SessionId, community);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.CommunityStewards,
                    community.CommunityStewards,
                    CommunityStewardInsertByBatch.Execute,
                    CommunityStewardUpdateByBatch.Execute,
                    CommunityStewardDeleteByBatch.Execute);
            });
        });
    }

    public async Task<Response> Remove(Request<Community> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var community = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await CommunityDelete.Execute(connection, transaction, request.SessionId, community);
            });
        });
    }

    public async Task<Response<IList<Selectable>>> FetchDistrictContacts(Request<Community> request)
    {
        return await wrappers.Try<IList<Selectable>>(request, async response =>
            await CommunitySelectableDistrictContacts.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<Community?>> FetchSchoolSelections(Request<Community> request)
    {
        return await wrappers.Try<Community?>(request, async response =>
        {
            var community = request.Value;
            community.Schools = (await SchoolSelectSelectionsByCommunity.Execute(Connection, request.SessionId, community.Id)).ToObservable();
            return community;
        });
    }

    public async Task<Response> SaveSchoolSelections(Request<Community> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var community = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SchoolUpdateSelectionsByCommunity.Execute(connection, transaction, request.SessionId, community);
            });
        });
    }
}